using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using KKManager.Data.Cards;
using KKManager.Data.ExtData.Deserializers;
using KKManager.Data.Scenes;
using MessagePack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Data.Scenes
{
    [TestClass]
    public class SceneLoaderTests
    {
        [TestMethod]
        public void ParsesUniversalAutoResolverGuidsFromSceneData()
        {
            var first = MessagePackSerializer.Serialize(new SideloaderExtDataDeserializer.ResolveInfo { GUID = "example.first" });
            var second = MessagePackSerializer.Serialize(new SideloaderExtDataDeserializer.ResolveInfo { GUID = "example.second" });
            var pluginData = new PluginData
            {
                version = 0,
                data = new Dictionary<string, object> { ["info"] = new List<byte[]> { first, second, first } }
            };
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");
            try
            {
                File.WriteAllBytes(path, Combine(Encoding.UTF8.GetBytes("scene-prefix-com.bepis.sideloader.universalautoresolver"), MessagePackSerializer.Serialize(pluginData)));
                Assert.IsTrue(SceneLoader.TryParseScene(new FileInfo(path), out var scene));
                CollectionAssert.AreEquivalent(new[] { "example.first", "example.second" }, scene.UsedZipmods);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [TestMethod]
        public void SceneWithoutUniversalAutoResolverDataHasNoRequirements()
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");
            try
            {
                File.WriteAllBytes(path, new byte[] { 1, 2, 3 });
                Assert.IsTrue(SceneLoader.TryParseScene(new FileInfo(path), out var scene));
                Assert.AreEqual(0, scene.UsedZipmods.Length);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        private static byte[] Combine(byte[] first, byte[] second)
        {
            var combined = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, combined, 0, first.Length);
            Buffer.BlockCopy(second, 0, combined, first.Length, second.Length);
            return combined;
        }
    }
}
