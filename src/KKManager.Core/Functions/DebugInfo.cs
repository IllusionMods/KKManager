using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace KKManager.Functions
{
    public static class DebugInfo
    {
        public static void GenerateDebugInfo(string atPath)
        {
            var epoch = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString();
            var tempDebugDir = Directory.CreateDirectory(Path.Combine(atPath, $"DebugInfoTemp {epoch}"));

            try
            {
                var fileTree = GenerateFileTree();
                File.WriteAllText(Path.Combine(tempDebugDir.FullName, "Files.txt"), fileTree);

                ZipPluginsAndConfig(Path.Combine(tempDebugDir.FullName, "Bepin.zip"));

                GetLogs(tempDebugDir);

                ZipFile.CreateFromDirectory(tempDebugDir.FullName, Path.Combine(atPath,
                    $"{InstallDirectoryHelper.GameType.GetFancyGameName()} Debug Info {epoch}.zip"));
            }
            finally
            {
                tempDebugDir.Delete(true);
            }
        }

        private static string GenerateFileTree()
        {
            var resultTree = new StringBuilder();

            var baseDirectories = InstallDirectoryHelper.GameDirectory.GetDirectories();

            foreach (var dI in baseDirectories)
            {
                var dirName = dI.Name;
                // get everything for now
                // if (!IsInterestingDirectory(dirName)) continue;
                resultTree.AppendLine(dirName);
                RecurseDirectories(dI, 1, resultTree);
                resultTree.AppendLine();
            }

            ListFilesInDirectory(InstallDirectoryHelper.GameDirectory, 0, resultTree);

            return resultTree.ToString();
        }

        private static void RecurseDirectories(DirectoryInfo dirInfo, int depth, StringBuilder resultTree)
        {
            var dirs = dirInfo.GetDirectories();

            ListFilesInDirectory(dirInfo, depth, resultTree);

            foreach (var dI in dirs)
            {
                for (var i = 1; i < depth; i++)
                {
                    resultTree.Append("  ");
                }

                resultTree.Append("|-");
                resultTree.AppendLine(dI.Name);

                RecurseDirectories(dI, depth + 1, resultTree);
            }
        }

        private static void ListFilesInDirectory(DirectoryInfo dirInfo, int depth, StringBuilder resultTree)
        {
            var files = dirInfo.GetFiles();

            var inBaseDirectory = depth == 0;

            var nameLength = 0;
            foreach (var file in files)
            {
                if (file.Name.Length > nameLength)
                {
                    nameLength = file.Name.Length;  //This doesn't handle multibyte Unicode characters correctly
                }
            }

            foreach (var file in files)
            {
                if (!inBaseDirectory)
                {
                    for (var i = 1; i < depth; i++)
                    {
                        resultTree.Append("  ");
                    }

                    resultTree.Append(" |-");
                }
                resultTree.Append(file.Name.PadRight(nameLength));
                resultTree.Append(" ");
                resultTree.Append(file.LastWriteTime);
                resultTree.Append(" ");
                resultTree.Append(file.Length / 1024);
                resultTree.AppendLine("kb");
            }
        }

        //static bool IsInterestingDirectory(string path)
        //{
        //    return string.Equals(path, "abdata") || string.Equals(path, "lib") || path.Contains("_Data");
        //}

        private static void ZipPluginsAndConfig(string outputFilePathAndName)
        {
            using (var zipStream = new FileStream(outputFilePathAndName, FileMode.Create))
            {
                using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create))
                {
                    try
                    {
                        var pluginsDirInfo = new DirectoryInfo(Path.Combine(InstallDirectoryHelper.PluginPath.FullName, "plugins"));
                        foreach (var file in pluginsDirInfo.EnumerateFiles("*.dll", SearchOption.AllDirectories))
                        {
                            zip.CreateEntryFromFile(file.FullName, RelativePath(file.FullName, pluginsDirInfo.FullName));
                        }
                    }
                    catch (Exception) { /* no plugins directory, oh well */ }

                    try
                    {
                        var configDirInfo = new DirectoryInfo(Path.Combine(InstallDirectoryHelper.PluginPath.FullName, "config"));
                        foreach (var file in configDirInfo.EnumerateFiles("*", SearchOption.AllDirectories))
                        {
                            zip.CreateEntryFromFile(file.FullName, RelativePath(file.FullName, configDirInfo.FullName));
                        }
                    }
                    catch (Exception) {/* same as above */ }
                }
            }
        }

        private static string RelativePath(string fullPath, string pathRelativeTo)
        {
            var baseUri = new Uri(pathRelativeTo);
            var fullUri = new Uri(fullPath);

            var relativeUri = baseUri.MakeRelativeUri(fullUri);

            return relativeUri.ToString().Replace('/', '\\');
        }

        private static void GetLogs(DirectoryInfo debugDirInfo)
        {
            var logOutputFiles = InstallDirectoryHelper.FindLogFiles(true);

            foreach (var logFile in logOutputFiles)
                logFile.CopyTo(Path.Combine(debugDirInfo.FullName, logFile.Name), true);
        }
    }
}
