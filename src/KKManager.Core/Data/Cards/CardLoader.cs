using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Data.Cards.AI;
using KKManager.Data.Cards.EC;
using KKManager.Data.Cards.KK;
using KKManager.Data.Plugins;
using KKManager.Data.Zipmods;

namespace KKManager.Data.Cards
{
    public static class CardLoader
    {
        public static IObservable<Card> ReadCards(DirectoryInfo path, CancellationToken cancellationToken)
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
                        foreach (var file in path.EnumerateFiles("*.png", SearchOption.TopDirectoryOnly))
                        {
                            if (cancellationToken.IsCancellationRequested) break;
                            try
                            {
                                if (ParseCard(file, out Card card))
                                    s.OnNext(card);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Failed to parse card [{file}] with an error: {ex.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to read cards from directory [{path.FullName}] with an error: {ex}");
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
            var pluginGuids = cardExtDatas.SelectMany(x => x.Value.RequiredPluginGUIDs);
            var missingPlugs = pluginGuids.Where(x => allPlugins.All(p => x != p.Guid)).Distinct().ToArray();
            if (missingPlugs.Length > 0)
                Console.WriteLine(card.Location.Name + " requires plugins that are missing: " + string.Join("; ", missingPlugs));

            var zipmodGuids = cardExtDatas.SelectMany(x => x.Value.RequiredZipmodGUIDs);
            var missingZipmods = zipmodGuids.Where(x => allZipmods.All(p => x != p.Guid)).Distinct().ToArray();
            if (missingZipmods.Length > 0)
                Console.WriteLine(card.Location.Name + " requires zipmods that are missing: " + string.Join("; ", missingZipmods));
        }

        private static bool ParseCard(FileInfo file, out Card card)
        {
            card = null;

            using (var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new BinaryReader(stream))
            {
                var pngEnd = Utility.SearchForPngEnd(stream);

                if (pngEnd == -1 || pngEnd >= stream.Length)
                    return false;

                stream.Position = pngEnd;

                try
                {
                    var loadProductNo = reader.ReadInt32();
                    if (loadProductNo > 100)
                    {
                        //return false;
                    }

                    var marker = reader.ReadString();
                    var gameType = GetGameType(marker);

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

                        case CardType.Unknown:
                            card = null;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException($"GameType={gameType} is not supported");
                    }
                }
                catch (EndOfStreamException e)
                {
                    throw new IOException("The card is corrupted or in an unknown format", e);
                }

                if (card?.Extended != null) ExtData.ExtDataParser.DeserializeInPlace(card.Extended);

                return card != null;
            }
        }

        private static CardType GetGameType(string marker)
        {
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
                // todo differnt format, saved at very end of data
                //case "【KStudio】":
                //    return CardType.KoikatuStudio;
                default:
                    return CardType.Unknown;
            }
        }
    }
}