﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using KKManager.Util;
using MessagePack;

namespace KKManager.Data.Cards.AI
{
    public class AiCard : Card
    {
        public override string Name => Parameter == null ? base.Name : $"{Parameter.fullname}";
        public override CharaSex Sex => Parameter == null ? CharaSex.Unknown : Parameter.sex == 0 ? CharaSex.Male : CharaSex.Female;
        public override string PersonalityName => GetPersonalityName(Parameter?.personality ?? -1);

        [ReadOnly(true)] public ChaFileParameter Parameter { get; private set; }
        [ReadOnly(true)] public ChaFileParameter2 Parameter2 { get; private set; }
        [ReadOnly(true)] public ChaFileGameInfo Gameinfo { get; private set; }
        [ReadOnly(true)] public ChaFileGameInfo2 Gameinfo2 { get; private set; }

        public override Image GetCardFaceImage()
        {
            return null;
        }

        public static AiCard ParseAiChara(FileInfo file, BinaryReader reader, CardType gameType)
        {
            var loadVersion = new Version(reader.ReadString());
            if (0 > new Version("0.0.0").CompareTo(loadVersion))
            {
                //return null;
            }

            var language = reader.ReadInt32();
            var userID = reader.ReadString();
            var dataID = reader.ReadString();
            var count = reader.ReadInt32();
            var bytes = reader.ReadBytes(count);
            var blockHeader = MessagePackSerializer.Deserialize<BlockHeader>(bytes);
            var num = reader.ReadInt64();
            var position = reader.BaseStream.Position;

            Dictionary<string, PluginData> extData = null;
            var info = blockHeader.SearchInfo(ChaFileExtended.BlockName);
            if (info != null)
            {
                reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
                var parameterBytes = reader.ReadBytes((int)info.size);
                extData = MessagePackSerializer.Deserialize<Dictionary<string, PluginData>>(parameterBytes);
            }
            var extendedSize = info != null ? Util.FileSize.FromBytes((int)info.size) : Util.FileSize.Empty;

            var card = new AiCard(file, gameType, extData, extendedSize, loadVersion)
            {
                Language = language,
                UserID = userID,
                DataID = dataID
            };

            void SetData<T>(string blockName, Action<T> set)
            {
                info = blockHeader.SearchInfo(blockName);
                if (info != null)
                {
                    reader.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);

                    var parameterBytes = reader.ReadBytes((int)info.size);
                    var parameter = MessagePackSerializer.Deserialize<T>(parameterBytes);

                    var versionProp = typeof(T).GetProperty("version", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                    Debug.Assert(versionProp != null, nameof(versionProp) + " != null");
                    if (versionProp != null) versionProp.SetValue(parameter, new Version(info.version), null);

                    set(parameter);
                }
            }

            SetData<ChaFileParameter>(ChaFileParameter.BlockName, x => card.Parameter = x);
            SetData<ChaFileParameter2>(ChaFileParameter2.BlockName, x => card.Parameter2 = x);
            SetData<ChaFileGameInfo>(ChaFileGameInfo.BlockName, x => card.Gameinfo = x);
            SetData<ChaFileGameInfo2>(ChaFileGameInfo2.BlockName, x => card.Gameinfo2 = x);

            return card;
        }

        public string GetPersonalityName(int personality)
        {
            if (Sex == CharaSex.Male) return "Male";

            string[] personalityLookup =
            {
                "Emotionless and stoic",
                "Friendly and gentle",
                "Confident and aware",
                "Selfish and spoiled",
                "Lazy and sluggish",
                "Positive and cheerful"
            };

            if (personality < 0 || personality > 90) return "Invalid";

            if (personalityLookup.Length > personality)
                return personalityLookup[personality];

            return KKManager.Properties.Resources.Unknown;
        }

        public AiCard(FileInfo cardFile, CardType type, Dictionary<string, PluginData> extended, FileSize extendedSize, Version loadVersion) : base(cardFile, type, extended, extendedSize, loadVersion)
        {
        }
    }
}