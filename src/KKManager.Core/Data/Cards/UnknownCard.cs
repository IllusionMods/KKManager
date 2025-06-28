using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace KKManager.Data.Cards
{
    public class UnknownCard : Card
    {
        public override string Name => "Invalid / Unsupported";
        public override CharaSex Sex => CharaSex.Unknown;
        public override string PersonalityName => "";

        public UnknownCard(FileInfo cardFile, string errorMessage, Exception error = null)
            : base(cardFile, CardType.Unknown, new Dictionary<string, PluginData>(), Util.FileSize.Empty, new Version(0, 0))
        {
            ErrorMessage = errorMessage;
            Error = error;
        }

        public string ErrorMessage { get; }
        public Exception Error { get; }

        public override Image GetCardFaceImage()
        {
            return null;
        }
    }
}