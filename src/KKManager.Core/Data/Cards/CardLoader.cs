using System;
using System.IO;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Data.Cards.AI;
using KKManager.Data.Cards.EC;
using KKManager.Data.Cards.KK;

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
                            throw new ArgumentOutOfRangeException(gameType + " is not supported");
                    }
                }
                catch (EndOfStreamException)
                {
                    return false;
                }

                if (card?.Extended != null) ExtData.ExtDataParser.Parse(card.Extended); // todo cache results or lazy run in background?

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