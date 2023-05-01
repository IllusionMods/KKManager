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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Data.Cards.AI;
using KKManager.Data.Cards.EC;
using KKManager.Data.Cards.KK;
using KKManager.Data.Cards.KKS;
using KKManager.Data.Cards.RG;
using KKManager.Data.Plugins;
using KKManager.Data.Zipmods;
using KKManager.Data.Game;
using MessagePack;
using libpngsharp;
using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;

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
                        foreach (FileInfo file in path.EnumerateFiles("*.png", searchOption))
                        {
                            if (cancellationToken.IsCancellationRequested) break;
                            if (TryParseCard(file, out var card)) s.OnNext(card);
                        }
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
            var pluginGuids = cardExtDatas.SelectMany(x => x.Value.RequiredPluginGUIDs);
            var missingPlugs = pluginGuids.Where(x => allPlugins.All(p => x != p.Guid)).Distinct().ToArray();
            if (missingPlugs.Length > 0)
            {
                card.MissingPlugins = missingPlugs;
                //Console.WriteLine(card.Location.Name + " requires plugins that are missing: " + string.Join("; ", missingPlugs));
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
                /*(IllusionObject, CardErrorCode) result = LoadCard(file);
                card = result.Item1;
                if(result.Item2 < 0)
                {
                    Console.WriteLine($"Error parsing card [{file}] with code {result.Item2}");
                }
                return card != null;*/
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to parse card [{file}] with an error: {ex.Message}");
                card = null;
                return false;
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
                case "【KoiKatuCharaSun】":
                    return CardType.KoikatsuSunshine;
                case "【RG_Chara】":
                    return CardType.RoomGirl;
                // todo differnt format, saved at very end of data
                //case "【KStudio】":
                //    return CardType.KoikatuStudio;
                default:
                    return CardType.Unknown;
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
                    var gameType = GetGameType(marker);

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

                        case CardType.Unknown:
                        default:
                            throw new ArgumentOutOfRangeException($"GameType={gameType} is not supported");
                    }

                    if (card.Extended != null)
                        ExtData.ExtDataParser.DeserializeInPlace(card.Extended);

                    LoadCard(file, card);
                    return card;
                }
                catch (EndOfStreamException e)
                {
                    throw new IOException("The card is corrupted or in an unknown format", e);
                }
            }
        }
        
        public enum CardErrorCode
        {
            NotFound = -12,
            NoDataFound,
            InvalidProductNumber,
            UnknownGame,
            UnknownVersion,
            CustomVersionMismatch,
            CoordinateVersionMismatch,
            ParameterVersionMismatch,
            GameInfoVersionMismatch,
            Parameter2VersionMismatch,
            GameInfo2VersionMismatch,
            EndOfStreamError,
            ValidCard
        }

        public static string PeekString(BinaryReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            var startPosition = reader.BaseStream.Position;
            var result = reader.ReadString();
            reader.BaseStream.Seek(startPosition, SeekOrigin.Begin);
            return result;
        }

        public static string PeekString(BinaryReader reader, int length)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (length <= 0)
            {
                return string.Empty;
            }

            byte[] bytes = reader.ReadBytes(length);
            string result = Encoding.UTF8.GetString(bytes);
            reader.BaseStream.Seek(-length, SeekOrigin.Current);
            return result;
        }

        // created for test purposes to basically cast ChaFile as Card.
        public static void CopyProperties(object source, object destination)
        {
            Type sourceType = source.GetType();
            Type destinationType = destination.GetType();

            foreach (var sourceProperty in sourceType.GetProperties())
            {
                var destinationProperty = destinationType.GetProperty(sourceProperty.Name);
                if (destinationProperty != null && destinationProperty.CanWrite && destinationProperty.PropertyType == sourceProperty.PropertyType)
                {
                    object value = sourceProperty.GetValue(source, null);
                    destinationProperty.SetValue(destination, value, null);
                }
            }
        }


        public static (IllusionObject, CardErrorCode) LoadCard(FileInfo pngFile, Card card)
        {
            if (!pngFile.Exists)
            {
                return (null, CardErrorCode.NotFound);
            }

            var gameObject = new IllusionObject();
            GameName gameName;

            using (var stream = pngFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Create a PNG reader
                PngReader reader = new PngReader(stream);

                // Get the chunks list
                ChunksList chunksList = reader.GetChunksList();

                // Find the IEND chunk
                PngChunk iendChunk = chunksList.GetById1("IEND", true);
                if (iendChunk == null)
                {
                    return (null, CardErrorCode.NoDataFound);
                }
                // Calculate the offset for the custom chunk
                int customChunkOffset = (int)iendChunk.Offset + 12;

                // Seek to the custom chunk offset in the stream
                stream.Seek(customChunkOffset, SeekOrigin.Begin);

                // Read the custom chunk data using a new binary reader
                using (BinaryReader br = new BinaryReader(stream))
                {
                    // long illusionChunkLength = br.ReadInt32(); // apparently illusion doesn't follow the png header format?
                    try
                    {
                        string chunkType = PeekString(br);
                        if (!Consts.gameNameByChunkDict.ContainsKey(chunkType))
                        {
                            // do honey select 1 stuff.
                        }
                        long loadProductNo = br.ReadInt32();
                        if (loadProductNo > 100)
                        {
                            return (null, CardErrorCode.InvalidProductNumber);
                        }

                        Version loadVersion = new Version(br.ReadString());
                        Version chaFileVersion = gameObject.Get<IllusionObject>("ChaFileDefine").Get<Version>("ChaFileVersion");
                        if (chaFileVersion != null && loadVersion > chaFileVersion)
                        {
                            return (null, CardErrorCode.UnknownVersion);
                        }

                        chunkType = br.ReadString();
                        if(!Consts.gameNameByChunkDict.ContainsKey(chunkType))
                        {
                            return (null, CardErrorCode.UnknownGame);
                        }

                        gameName = Consts.gameNameByChunkDict[chunkType];
                        if (gameName.Equals("HS2") || gameName.Equals("EC"))
                        {
                            long language = br.ReadInt32();
                            string userID = br.ReadString();
                            string dataID = br.ReadString();
                        }
                        else if(gameName.Equals("KK") || gameName.Equals("KKS"))
                        {
                            int facePngLength = br.ReadInt32();
                            if (facePngLength > 0)
                            {
                                //this.facePngData = reader.ReadBytes(num);
                                br.ReadBytes(facePngLength);
                            }
                        }
                        if (gameName.Equals("EC"))
                        {
                            int someECLen = br.ReadInt32();
                            var hsPackage = new HashSet<int>();
                            for (int i = 0; i < someECLen; i++)
                            {
                                hsPackage.Add(br.ReadInt32());
                            }
                        }

                        int blockHeaderSize = br.ReadInt32();
                        BlockHeader blockHeader = MessagePackSerializer.Deserialize<BlockHeader>(br.ReadBytes(blockHeaderSize));
                        long seekNum = br.ReadInt64();
                        long position = br.BaseStream.Position;

                        gameObject.Acquire(gameName); // Load our IllusionObject with game-specific data.
                        SortedDictionary<string, object> blocks = BlockHelper.GetValidBlockNames(gameName);
                        int blockErrCode = -7;
                        foreach (var blockName in blocks)
                        {
                            BlockHeader.Info info = blockHeader.SearchInfo(blockName.Key);
                            if (info != null)
                            {
                                Version version = new Version(info.version);
                                string blockShortName = BlockHelper.blockShortNames[blockName.Key];
                                string blockVersionMethod = BlockHelper.blockVersionMethodName(blockName.Key);
                                Version blockVersion = (Version)gameObject.Get<IllusionObject>("ChaFileDefine").Call(blockVersionMethod);
                                if (blockVersion > version)
                                {
                                    return (null, (CardErrorCode)(blockErrCode));
                                }

                                br.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
                                byte[] data = br.ReadBytes((int)info.size);
                                card[BlockHelper.blockShortNames[blockName.Key]] = data;
                                string blockMethodName = BlockHelper.blockMethods[blockName.Key];
                                gameObject.Get<IllusionObject>("ChaFile").Call(blockMethodName, data, version);
                            }
                            blockErrCode++;
                        }

                        br.BaseStream.Seek(position + seekNum, SeekOrigin.Begin);
                    }
                    catch (EndOfStreamException)
                    {
                        return (null, CardErrorCode.EndOfStreamError);
                    }
                }
            }

            return (gameObject, CardErrorCode.ValidCard);
        }
    }
}
