using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Data.Cards.AI;
using KKManager.Data.Cards.EC;
using KKManager.Data.Cards.HC;
using KKManager.Data.Cards.KK;
using KKManager.Data.Cards.KKS;
using KKManager.Data.Cards.RG;
using KKManager.Data.Plugins;
using KKManager.Data.Zipmods;
using KKManager.Util;

namespace KKManager.Data.Cards
{
    public static class CardLoader
    {
        public static IObservable<Card> ReadCards(DirectoryInfo path, SearchOption searchOption, CancellationToken cancellationToken)
        {
            var s = new ReplaySubject<Card>();

            if (cancellationToken.IsCancellationRequested)
            {
                s.OnCompleted();
                return s;
            }

            if (!path.Exists)
            {
                MessageBox.Show($"The card directory \"{path.FullName}\" doesn't exist or is inaccessible.", "Load cards",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                s.OnCompleted();
            }
            else
            {
                void ReadCardsFromDir()
                {
                    try
                    {
                        Parallel.ForEach(path.EnumerateFiles("*.png", searchOption),
                                         new ParallelOptions { CancellationToken = cancellationToken },
                                         file =>
                        {
                            if (TryParseCard(file, out var card)) s.OnNext(card);
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to read cards from directory [{path.FullName}] with an error: {ex.ToStringDemystified()}");
                    }

                    s.OnCompleted();
                }

                try
                {
                    var readCardTask = Task.Run(ReadCardsFromDir, cancellationToken);

                    Task.WhenAll(readCardTask,
                            SideloaderModLoader.Zipmods.ToTask(cancellationToken),
                            PluginLoader.Plugins.ToTask(cancellationToken))
                        .ContinueWith(t =>
                        {
                            try
                            {
                                var allPlugins = PluginLoader.Plugins.ToEnumerable().ToList();
                                var allZipmods = SideloaderModLoader.Zipmods.ToEnumerable().ToList();
                                foreach (var card in s.ToEnumerable())
                                    CheckIfRequiredModsExist(card, allPlugins, allZipmods);
                            }
                            catch (TargetInvocationException ex)
                            {
                                if (ex.InnerException is OperationCanceledException) return;
                                throw;
                            }

                            // Free up memory stuck in LOH from loading cards, can be fairly substantial
                            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
                        }, cancellationToken);
                }
                catch (OperationCanceledException) { }
            }

            return s;
        }

        private static void CheckIfRequiredModsExist(Card card, List<PluginInfo> allPlugins, List<SideloaderModInfo> allZipmods)
        {
            if (card.Extended == null) return;

            var cardExtDatas = card.Extended.Where(x => x.Value != null).ToList();

            var extGroups = cardExtDatas.ToLookup(x => x.Value.RequiredPluginGUIDs.Count > 0);

            var missingPlugs = extGroups[true].SelectMany(x => x.Value.RequiredPluginGUIDs).Where(x => allPlugins.All(p => x != p.Guid)).Distinct().ToArray();
            if (missingPlugs.Length > 0)
            {
                card.MissingPlugins = missingPlugs;
                //Console.WriteLine(card.Location.Name + " requires plugins that are missing: " + string.Join("; ", missingPlugs));
            }

            var allExtCandidates = allPlugins.SelectMany(z => z.ExtDataGuidCandidates ?? Enumerable.Empty<string>()).ToHashSet();
            var missingPlugsMaybe = extGroups[false].Where(x => !allExtCandidates.Contains(x.Key)).Select(x => x.Key).Distinct().ToArray();
            if (missingPlugsMaybe.Length > 0)
            {
                card.MissingPluginsMaybe = missingPlugsMaybe;
            }

            var zipmodGuids = cardExtDatas.SelectMany(x => x.Value.RequiredZipmodGUIDs);
            var missingZipmods = zipmodGuids.Where(x => allZipmods.All(p => x != p.Guid)).Distinct().ToArray();
            if (missingZipmods.Length > 0)
            {
                card.MissingZipmods = missingZipmods;
                //Console.WriteLine(card.Location.Name + " requires zipmods that are missing: " + string.Join("; ", missingZipmods));
            }
        }

        public static bool TryParseCard(FileInfo file, out Card card)
        {
            try
            {
                card = ParseCard(file);
                return card != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to parse card [{file}] with an error: {ex.Message}");
                card = null;
                return false;
            }
        }

        public static Card ParseCard(FileInfo file)
        {
            if (!file.Exists) return null;

            using (var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new BinaryReader(stream))
            {
                var pngEnd = Utility.SearchForPngEnd(stream);

                if (pngEnd == -1 || pngEnd >= stream.Length)
                    return null;

                stream.Position = pngEnd;

                try
                {
                    var loadProductNo = reader.ReadInt32();
                    //if (loadProductNo > 100)
                    //{
                    //    return null;
                    //}

                    var marker = reader.ReadString();
                    var gameType = GetGameType(marker, true);

                    Card card;
                    switch (gameType)
                    {
                        case CardType.Koikatu:
                        case CardType.KoikatsuParty:
                        case CardType.KoikatsuPartySpecialPatch:
                            card = KoiCard.ParseKoiChara(file, reader, gameType);
                            break;

                        case CardType.EmotionCreators:
                            card = EmoCard.ParseEmoChara(file, reader, gameType);
                            break;

                        case CardType.AiSyoujyo:
                            card = AiCard.ParseAiChara(file, reader, gameType);
                            break;

                        case CardType.KoikatsuSunshine:
                            card = KoiSunCard.ParseKoiChara(file, reader, gameType);
                            break;

                        case CardType.RoomGirl:
                            card = RoomGirlCard.ParseRGChara(file, reader, gameType);
                            break;
                        case CardType.HoneyCome:
                        case CardType.HoneyComeccp:
                            card = HoneyCoomCard.ParseHCPChara(file, reader, gameType);
                            break;

                        case CardType.Unknown:
                        default:
                            throw new ArgumentOutOfRangeException($"GameType={gameType} is not supported");
                    }

                    if (card.Extended != null)
                    {
                        ExtData.ExtDataParser.DeserializeInPlace(card.Extended);

                        // Remove any byte[] extended data from the card, as it's not useful anymore and takes up a lot of memory
                        foreach (var pluginData in card.Extended.Values)
                        {
                            if (pluginData?.Data == null) continue;

                            foreach (var data in pluginData.Data.Where(x => x.Value is byte[]).ToList())
                            {
                                // Keep the key to know what data was there, but clear the value
                                pluginData.Data[data.Key] = Array.Empty<byte>();
                            }
                        }
                    }

                    return card;
                }
                catch (EndOfStreamException e)
                {
                    throw new IOException("The card is corrupted or in an unknown format", e);
                }
            }
        }

        private static CardType GetGameType(string marker, bool throwOnUnknown)
        {
            if (marker == null) throw new ArgumentNullException(nameof(marker));
            switch (marker)
            {
                case "【KoiKatuChara】":
                    return CardType.Koikatu;
                case "【KoiKatuCharaS】":
                    return CardType.KoikatsuParty;
                case "【KoiKatuCharaSP】":
                    return CardType.KoikatsuPartySpecialPatch;
                case "【EroMakeChara】":
                    return CardType.EmotionCreators;
                case "【AIS_Chara】":
                    return CardType.AiSyoujyo;
                case "【KoiKatuCharaSun】":
                    return CardType.KoikatsuSunshine;
                case "【RG_Chara】":
                    return CardType.RoomGirl;
                case "【HCChara】":
                    return CardType.HoneyCome;
                case "【HCPChara】":
                    return CardType.HoneyComeccp;
                // todo differnt format, saved at very end of data
                //case "【KStudio】":
                //    return CardType.KoikatuStudio;
                default:
                    if (throwOnUnknown)
                        throw new ArgumentOutOfRangeException($"Unknown game tag: {PathTools.SanitizeFileName(marker.Left(20))}");
                    else
                        return CardType.Unknown;
            }
        }
    }
}