using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace KKManager.Functions
{
    public class DebugInfo
    {
        public static void GenerateDebugInfo()
        {
            DirectoryInfo tempDebugDir = Directory.CreateDirectory("DebugInfoTemp");

            try
            {
                string fileTree = GenerateFileTree();
                File.WriteAllText(Path.Combine(tempDebugDir.FullName, "Files.txt"), fileTree);

                ZipPluginsAndConfig(Path.Combine(tempDebugDir.FullName, "Bepin.zip"));

                GetLogs(tempDebugDir);

                string epoch = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString();
                ZipFile.CreateFromDirectory(tempDebugDir.FullName, Path.Combine(InstallDirectoryHelper.GameDirectory.FullName,
                    $"{InstallDirectoryHelper.GetFancyGameName(InstallDirectoryHelper.GameType)} Debug Info {epoch}.zip"));
            }           
            finally
            {
                tempDebugDir.Delete(true);
            }
        }

        static string GenerateFileTree()
        {
            StringBuilder resultTree = new StringBuilder();

            DirectoryInfo[] baseDirectories = InstallDirectoryHelper.GameDirectory.GetDirectories();

            foreach (DirectoryInfo dI in baseDirectories)
            {
                string dirName = dI.Name;
                // get everything for now
                // if (!IsInterestingDirectory(dirName)) continue;
                resultTree.AppendLine(dirName);
                RecurseDirectories(dI, 1, resultTree);
                resultTree.AppendLine();
            }

            ListFilesInDirectory(InstallDirectoryHelper.GameDirectory, 0, resultTree);

            return resultTree.ToString();
        }

        static void RecurseDirectories(DirectoryInfo dirInfo, int depth, StringBuilder resultTree)
        {
            DirectoryInfo[] dirs = dirInfo.GetDirectories();

            ListFilesInDirectory(dirInfo, depth, resultTree);

            foreach (DirectoryInfo dI in dirs)
            {
                for (int i = 1; i < depth; i++)
                {
                    resultTree.Append("  ");
                }

                resultTree.Append("|-");
                resultTree.AppendLine(dI.Name);

                RecurseDirectories(dI, depth + 1, resultTree);
            }
        }

        static void ListFilesInDirectory(DirectoryInfo dirInfo, int depth, StringBuilder resultTree)
        {
            FileInfo[] files = dirInfo.GetFiles();

            bool inBaseDirectory = depth == 0;

            int nameLength = 0;
            foreach (FileInfo file in files)
            {
                if (file.Name.Length > nameLength) 
                { 
                    nameLength = file.Name.Length;  //This doesn't handle multibyte Unicode characters correctly
                }
            }

            foreach (FileInfo file in files)
            {
           //     if (!inBaseDirectory)
                {
                    for (int i = 1; i < depth; i++)
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

        static void ZipPluginsAndConfig(string outputFilePathAndName)
        {
            using (FileStream zipStream = new FileStream(outputFilePathAndName, FileMode.Create))
            {
                using (ZipArchive zip = new ZipArchive(zipStream, ZipArchiveMode.Create))
                {
                    DirectoryInfo pluginsDirInfo = new DirectoryInfo(Path.Combine(InstallDirectoryHelper.PluginPath.FullName, "plugins"));
                    foreach (FileInfo file in pluginsDirInfo.EnumerateFiles("*.dll", SearchOption.AllDirectories))
                    {                       
                        zip.CreateEntryFromFile(file.FullName, RelativePath(file.FullName, pluginsDirInfo.FullName));
                    }

                    DirectoryInfo configDirInfo = new DirectoryInfo(Path.Combine(InstallDirectoryHelper.PluginPath.FullName, "config"));
                    foreach (FileInfo file in configDirInfo.EnumerateFiles("*", SearchOption.AllDirectories))
                    {
                        zip.CreateEntryFromFile(file.FullName, RelativePath(file.FullName, configDirInfo.FullName));
                    }
                }
            }
        }

        static string RelativePath(string fullPath, string pathRelativeTo)
        {
            Uri baseUri = new Uri(pathRelativeTo);
            Uri fullUri = new Uri(fullPath);

            Uri relativeUri = baseUri.MakeRelativeUri(fullUri);

            return relativeUri.ToString().Replace('/', '\\');
        }

        static void GetLogs(DirectoryInfo debugDirInfo)
        {
            FileInfo[] logOutputFiles = InstallDirectoryHelper.GameDirectory.EnumerateFiles("LogOutput.log*", SearchOption.AllDirectories).ToArray();
            FileInfo[] outputLogFiles = InstallDirectoryHelper.GameDirectory.EnumerateFiles("output_log.txt", SearchOption.AllDirectories).ToArray();

            if (outputLogFiles.Length == 0)
            {
                //We are probably on one of the games that logs to AppData/LocalLow, and the user hasn't configured Doorstop to redirect the logs.
                //For now just log that this happens instead of trying to traverse the AppData folder (since we'd have to determine the subfolder name in AppData anyway)
                Console.WriteLine("Could not locate output_log.txt! Check if redirect_output_log is set to true in doorstop_config.ini");
            }

            foreach (FileInfo logFile in logOutputFiles)
            {
                logFile.CopyTo(Path.Combine(debugDirInfo.FullName, logFile.Name), true);
            }

            foreach (FileInfo outputFile in outputLogFiles)
            {
                string uniqueName = $"{outputFile.Directory.Name} {outputFile.Name}"; //There may be be multiple output_logs so we have to differentiate them
                outputFile.CopyTo(Path.Combine(debugDirInfo.FullName, uniqueName), true);
            }
        }
    }
}
