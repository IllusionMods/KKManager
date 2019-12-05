﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using KKManager.Data.Cards.Internal;
using MessagePack;

namespace KKManager.Data.Cards
{
    [SuppressMessage("ReSharper", "UnusedVariable")]
    public class Card : IFileInfoBase
    {
        public string Name => Parameter == null ? "[Missing data]" : $"{Parameter.lastname} {Parameter.firstname}";
        public FileInfo Location { get; }
        public CardSource SourceGame { get; }

        public ChaFileParameter Parameter { get; private set; }
        public Dictionary<string, PluginData> Extended { get; private set; }

        public Image GetCardImage()
        {
            using (var stream = Location.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                return Image.FromStream(stream);
        }

        public Image GetCardFaceImage()
        {
            using (var stream = Location.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new BinaryReader(stream))
            {
                stream.Position = Utility.SearchForIEND(stream);
                var loadProductNo = reader.ReadInt32();
                var marker = reader.ReadString();
                var loadVersion = new Version(reader.ReadString());
                var faceLength = reader.ReadInt32();

                using (var memStream = new MemoryStream(reader.ReadBytes(faceLength)))
                    return Image.FromStream(memStream);
            }
        }

        private Card(FileInfo cardFile, CardSource sourceGame)
        {
            Location = cardFile;
            SourceGame = sourceGame;
        }

        public static bool TryParseCard(FileInfo file, out Card card)
        {
            card = null;

            using (var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new BinaryReader(stream))
            {
                var IEND = Utility.SearchForIEND(stream);

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
                    if (gameType == CardSource.Unknown)
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

        private static CardSource GetGameType(string marker)
        {
            switch (marker)
            {
                case "【KoiKatuChara】":
                    return CardSource.Koikatu;
                case "【KoiKatuCharaS】":
                    return CardSource.Party;
                case "【KoiKatuCharaSP】":
                    return CardSource.PartySpecialPatch;
                default:
                    return CardSource.Unknown;
            }
        }

        public enum CardSource
        {
            Unknown,
            Koikatu,
            Party,
            PartySpecialPatch,
        }
    }
}
