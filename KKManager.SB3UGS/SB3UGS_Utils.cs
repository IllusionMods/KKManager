using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace KKManager.SB3UGS
{
    public static class SB3UGS_Utils
    {
        public static int HashFileContents(string unity3DFilePath)
        {
            SB3UGS_Initializer.ThrowIfNotAvailable();
            return HashFileContentsImpl(unity3DFilePath);
        }

        private static int HashFileContentsImpl(string unity3DFilePath)
        {
            using (var parser = UnityPlugin.Plugins.OpenUnity3d(unity3DFilePath))
            using (var editor = new UnityPlugin.Unity3dEditor(parser, false))
            {
                var sbResult = new StringBuilder();
                var names = editor.GetAssetNames(false);
                for (var i = 0; i < names.Length; i++)
                {
                    var asset = editor.Parser.Cabinet.Components[i];
                    sbResult.AppendLine(asset.pathID.ToString(CultureInfo.InvariantCulture) + asset.classID1 + asset.classID2 + names[i]);
                    //Console.WriteLine("PathID=" + asset.pathID.ToString("D") + " id1=" + (int)asset.classID1 + "/" + asset.classID1 + " id2=" + asset.classID2 + " " + names[i]);
                }

                return sbResult.ToString().GetHashCode();
            }
        }

        public static bool FileIsAssetBundle(FileInfo file)
        {
            if (string.Equals(Path.GetExtension(file.Extension), ".unity3d", StringComparison.OrdinalIgnoreCase))
                return true;

            if (file.Length < 10) return false;

            var buffer = new byte[7];
            using (var fs = file.OpenRead())
            {
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
            }
            return Encoding.UTF8.GetString(buffer, 0, buffer.Length) == "UnityFS";
        }
    }
}