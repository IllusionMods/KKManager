using System.Collections.Generic;
using System.ComponentModel;
using KKManager.Util;
using MessagePack;

namespace KKManager.Data.Cards
{
    [MessagePackObject]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PluginData
    {
        [IgnoreMember]
        public int Version => version;
        [IgnoreMember]
        [TypeConverter(typeof(DictionaryTypeConverter<string, object>))]
        public Dictionary<string, object> Data => data;

        [Key(0)]
        public int version;
        [Key(1)]
        public Dictionary<string, object> data = new Dictionary<string, object>();

        public override string ToString()
        {
            return $"Version {version}; {data.Count} elements";
        }
    }
}