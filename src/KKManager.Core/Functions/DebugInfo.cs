using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace KKManager.Functions
{
    public class DebugInfo
    {
        public static void GenerateDebugInfo()
        {
            DirectoryInfo gameDir = InstallDirectoryHelper.GameDirectory;

            DirectoryInfo tempDebugDir = Directory.CreateDirectory("DebugInfoTemp");

            string fileTree = GenerateFileTree(gameDir);
            File.WriteAllText(Path.Combine(tempDebugDir.FullName, "Files.txt"), fileTree);

            ZipPluginsAndConfig(Path.Combine(tempDebugDir.FullName, "Bepin.zip"));

            string epoch = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString();
            ZipFile.CreateFromDirectory(tempDebugDir.FullName, Path.Combine(InstallDirectoryHelper.GameDirectory.FullName,
                $"{InstallDirectoryHelper.GetFancyGameName(InstallDirectoryHelper.GameType)} Debug Info {epoch}.zip"));

            tempDebugDir.Delete(true);
        }

        static string GenerateFileTree(DirectoryInfo gameDirectoryInfo)
        {
            StringBuilder resultTree = new StringBuilder();

            DirectoryInfo[] baseDiretories = gameDirectoryInfo.GetDirectories();

            foreach (DirectoryInfo dI in baseDiretories)
            {
                string dirName = dI.Name;
                if (!IsInterestingDirectory(dirName)) continue;
                resultTree.AppendLine(dirName);
                RecurseDirectories(dI, 1, resultTree);
                resultTree.AppendLine();
            }

            return resultTree.ToString();
        }

        static void RecurseDirectories(DirectoryInfo dirInfo, int depth, StringBuilder resultTree)
        {
            DirectoryInfo[] dirs = dirInfo.GetDirectories();

            AddFileEntry(dirInfo, depth, resultTree);

            foreach (DirectoryInfo dI in dirs)
            {
                string dirName = dI.Name;

                for (int i = 1; i < depth; i++)
                {
                    resultTree.Append("  ");
                }

                resultTree.Append("|-");
                resultTree.AppendLine(dirName);

                RecurseDirectories(dI, depth + 1, resultTree);
            }
        }

        static void AddFileEntry(DirectoryInfo dirInfo, int depth, StringBuilder resultTree)
        {
            FileInfo[] files = dirInfo.GetFiles();

            int nameLength = 0;
            foreach (FileInfo file in files)
            {
                if (file.Name.Length > nameLength) nameLength = file.Name.Length;
            }

            foreach (FileInfo file in files)
            {
                for (int i = 1; i < depth; i++) { resultTree.Append("  "); }
                resultTree.Append(" |-");
                resultTree.Append(file.Name.PadRight(nameLength));
                resultTree.Append(" ");
                resultTree.Append(file.LastWriteTime);
                resultTree.Append(" ");
                resultTree.Append(file.Length / 1024);
                resultTree.AppendLine("kb");
            }
        }

        static bool IsInterestingDirectory(string path)
        {
            return string.Equals(path, "abdata") || string.Equals(path, "lib") || path.Contains("_Data");
        }

        static void ZipPluginsAndConfig(string fileName)
        {

            using (FileStream zipStream = new FileStream(fileName, FileMode.Create))
            {
                using (ZipArchive zip = new ZipArchive(zipStream, ZipArchiveMode.Create))
                {
                    DirectoryInfo pluginsDirInfo = new DirectoryInfo(Path.Combine(InstallDirectoryHelper.PluginPath.FullName, "plugins"));
                    foreach (FileInfo file in pluginsDirInfo.GetFiles())
                    {
                        zip.CreateEntryFromFile(file.FullName, Path.Combine(file.Directory.Name, file.Name));
                    }

                    DirectoryInfo configDirInfo = new DirectoryInfo(Path.Combine(InstallDirectoryHelper.PluginPath.FullName, "config"));
                    foreach (FileInfo file in configDirInfo.GetFiles())
                    {
                        zip.CreateEntryFromFile(file.FullName, Path.Combine(file.Directory.Name, file.Name));
                    }
                }
            }
        }
    }
}
