using System;
using System.IO;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                                if (Card.TryParseCard(file, out Card card))
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
    }
}