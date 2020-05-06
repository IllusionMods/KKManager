using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Data.Cards.KK;
using MessagePack;

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
                                if (TryParseCard(file, out Card card))
                                    s.OnNext(card);
                            }
                            catch (SystemException ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                    }
                    catch (SystemException ex)
                    {
                        Console.WriteLine(ex);
                    }

                    s.OnCompleted();
                }

                Task.Run((Action)ReadCardsFromDir, cancellationToken);
            }

            return s;
        }

        private static bool TryParseCard(FileInfo file, out Card card)
        {
            card = null;

            using (var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new BinaryReader(stream))
            {
                var IEND = Utility.SearchForPngEnd(stream);

                if (IEND == -1 || IEND >= stream.Length)
                    return false;

                stream.Position = IEND;

                try
                {
                    var loadProductNo = reader.ReadInt32();
                    if (loadProductNo > 100)
                    {
                        return false;
                    }

                    var marker = reader.ReadString();
                    var gameType = GetGameType(marker);
                    if (gameType == CardType.Unknown)
                    {
                        return false;
                    }

                    var loadVersion = new Version(reader.ReadString());
                    if (0 > new Version("0.0.0").CompareTo(loadVersion))
                    {
                        return false;
                    }

                    var faceLength = reader.ReadInt32();
                    if (faceLength > 0)
                    {
                        //this.facePngData = reader.ReadBytes(num);
                        stream.Seek(faceLength, SeekOrigin.Current);
                    }

                    var count = reader.ReadInt32();
                    var bytes = reader.ReadBytes(count);
                    var blockHeader = MessagePackSerializer.Deserialize<BlockHeader>(bytes);
                    var num2 = reader.ReadInt64();
                    var position = reader.BaseStream.Position;

                    card = new Card(file, gameType);

                    var info = blockHeader.SearchInfo(ChaFileParameter.BlockName);
                    if (info != null)
                    {
                        var value = new Version(info.version);
                        if (0 <= ChaFileParameter.CurrentVersion.CompareTo(value))
                        {
                            reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
                            var parameterBytes = reader.ReadBytes((int)info.size);

                            card.Parameter = MessagePackSerializer.Deserialize<ChaFileParameter>(parameterBytes);
                            card.Parameter.ComplementWithVersion();
                        }
                    }

                    info = blockHeader.SearchInfo(ChaFileExtended.BlockName);
                    if (info != null)
                    {
                        reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
                        var parameterBytes = reader.ReadBytes((int)info.size);

                        card.Extended = MessagePackSerializer.Deserialize<Dictionary<string, PluginData>>(parameterBytes);
                    }

                    //reader.BaseStream.Seek(position + num2, SeekOrigin.Begin);
                }
                catch (EndOfStreamException)
                {
                    return false;
                }

                return true;
            }
        }

        private static CardType GetGameType(string marker)
        {
            switch (marker)
            {
                case "【KoiKatuChara】":
                    return CardType.Koikatu;
                case "【KoiKatuCharaS】":
                    return CardType.Party;
                case "【KoiKatuCharaSP】":
                    return CardType.PartySpecialPatch;
                default:
                    return CardType.Unknown;
            }
        }
    }
}