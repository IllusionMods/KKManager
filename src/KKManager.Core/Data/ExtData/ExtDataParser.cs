using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KKManager.Data.Cards;

namespace KKManager.Data.ExtData
{
    internal class ExtDataParser
    {
        private static readonly List<IExtDataDeserializer> _deserializers;

        static ExtDataParser()
        {
            var baseType = typeof(IExtDataDeserializer);
            var deserializerTypes = typeof(ExtDataParser).Assembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract && baseType.IsAssignableFrom(x));
            _deserializers = deserializerTypes.Select(Activator.CreateInstance).Cast<IExtDataDeserializer>().ToList();
            Console.WriteLine($"Found {_deserializers.Count} extended data deserializers");
        }

        public static void DeserializeInPlace(Dictionary<string, PluginData> extData)
        {
            foreach (var pluginData in extData)
            {
                foreach (var deserializer in _deserializers.Where(des => des.SupportedExtDataIDs.Any(id => id == pluginData.Key)))
                {
                    deserializer.DeserializeObjects(pluginData.Value);
                }
            }
        }
    }
}
