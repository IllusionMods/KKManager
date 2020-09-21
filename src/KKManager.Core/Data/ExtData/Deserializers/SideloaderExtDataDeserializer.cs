using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using KKManager.Data.Cards;
using KKManager.Data.Cards.AI;
using KKManager.Util;
using MessagePack;

namespace KKManager.Data.ExtData.Deserializers
{
    public class SideloaderExtDataDeserializer : IExtDataDeserializer
    {
        public const string PluginGUID = "com.bepis.bepinex.sideloader";

        /// <summary>
        /// Extended save ID
        /// </summary>
        public const string UARExtID = "com.bepis.sideloader.universalautoresolver";
        /// <summary>
        /// Extended save ID used in EmotionCreators once upon a time, no longer used but must still be checked for cards that still use it
        /// </summary>
        public const string UARExtIDOld = "EC.Core.Sideloader.UniversalAutoResolver";

        public IEnumerable<string> SupportedExtDataIDs { get; } = new[] { UARExtID, UARExtIDOld };
        public IEnumerable<string> RequiredPluginGUIDs { get; } = new[] { PluginGUID };

        public void DeserializeObjects(PluginData data)
        {
            foreach (var item in data.data.ToList())
            {
                switch (item.Key)
                {
                    case "info":
                        var resolveInfos = ((IEnumerable)item.Value).Cast<byte[]>().Select(ResolveInfo.Deserialize).ToList();
                        data.data[item.Key] = resolveInfos;
                        data.RequiredZipmodGUIDs.AddRange(resolveInfos.Select(x => x.GUID));
                        break;
                }
            }

            data.RequiredPluginGUIDs.Add(PluginGUID);
        }

        /// <summary>
        /// Contains information saved to the card for resolving ID conflicts
        /// </summary>
        [Serializable]
        [MessagePackObject]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class ResolveInfo
        {
            /// <summary>
            /// GUID of the mod as defined in the manifest.xml
            /// </summary>
            [Key("ModID")]
            public string GUID { get; set; }
            /// <summary>
            /// ID of the item as defined in the mod's list files
            /// </summary>
            [Key("Slot")]
            public int Slot { get; set; }
            /// <summary>
            /// Resolved item ID. IDs greater than 100000000 are resolved IDs belonging to Sideloader. Use the resolved ID (local slot) to look up the original ID (slot)
            /// </summary>
            [Key("LocalSlot")]
            public int LocalSlot { get; set; }
            /// <summary>
            /// Property of the object as defined in Sideloader's StructReference.
            /// If ever you need to know what to use for this, enable debug resolve info logging and see what Sideloader generates at the start of the game.
            /// </summary>
            [Key("Property")]
            public string Property { get; set; }
            /// <summary>
            /// ChaListDefine.CategoryNo. Typically only used for hard mod resolving in cases where the GUID is not known.
            /// </summary>
            [Key("CategoryNo")]
            public int CategoryNo { get; set; }

            internal static ResolveInfo Deserialize(byte[] data) => MessagePackSerializer.Deserialize<ResolveInfo>(data);

            internal byte[] Serialize() => MessagePackSerializer.Serialize(this);
        }
    }
}
