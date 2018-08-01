using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using KKManager.Cards.Data;

namespace KKManager.Cards
{
    public class CardLoader
    {
        private readonly List<Card> _cards;
        
        public CardLoader()
        {
            _cards = new List<Card>();
        }

        public void Read(DirectoryInfo path)
        {
            _cards.Clear();
            if (!path.Exists)
            {
                MessageBox.Show($"The card directory \"{path.FullName}\" doesn't exist or is inaccessible.", "Load cards",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                foreach (var file in path.EnumerateFiles("*.png", SearchOption.AllDirectories))
                {
                    if (Card.TryParseCard(file, out Card card))
                    {
                        _cards.Add(card);
                    }
                }
            }

            OnCardsChanged();
        }

        public event EventHandler CardsChanged;

        protected void OnCardsChanged()
        {
            CardsChanged?.Invoke(this, EventArgs.Empty);
        }

        public IReadOnlyCollection<Card> Cards
        {
            get { return _cards; }
        }
    }
}