using System.Collections.Generic;
using KKManager.Data.Cards;

namespace KKManager.Data.ExtData
{
    public interface IExtDataDeserializer
    {
        IEnumerable<string> SupportedExtDataIDs { get; }
        IEnumerable<string> RequiredPluginGUIDs { get; }
        void DeserializeObjects(PluginData data);
    }
}