using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using KKManager.Data.Cards.KK;

namespace KKManager.Data.Cards
{
    public class Card : IFileInfoBase
    {
        public string Name => Parameter == null ? "[Missing data]" : $"{Parameter.lastname} {Parameter.firstname}";
        public FileInfo Location { get; }
        public CardType TypeGame { get; }

        public ChaFileParameter Parameter { get; internal set; }
        public Dictionary<string, PluginData> Extended { get; internal set; }

        public virtual Image GetCardImage()
        {
            using (var stream = Location.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                return Image.FromStream(stream);
        }

        public virtual Image GetCardFaceImage()
        {
            using (var stream = Location.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Skip the first PNG and ignore it
                stream.Position = Utility.SearchForPngEnd(stream);
                // Find the second PNG and skip to it
                stream.Position = Utility.SearchForPngStart(stream);
                return Image.FromStream(stream);
            }
        }

        internal Card(FileInfo cardFile, CardType typeGame)
        {
            Location = cardFile;
            TypeGame = typeGame;
        }
    }

    public enum CardType
    {
        Unknown,
        Koikatu,
        Party,
        PartySpecialPatch,
    }
}
