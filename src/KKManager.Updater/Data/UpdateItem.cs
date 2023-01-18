using System;
using System.IO;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Functions;
using KKManager.Util;
using KKManager.Util.ProcessWaiter;

namespace KKManager.Updater.Data
{
    public class UpdateItem
    {
        /// <summary>
        /// Move downloaded file from temporary current path to the target path.
        /// Should return true on success, false if a normal FileInfo.MoveTo should be used instead.
        /// </summary>
        public delegate bool CustomMoveTo(FileInfo currentPath, FileInfo targetPath, UpdateItem item);

        public UpdateItem(FileInfo targetPath, IRemoteItem remoteFile, bool upToDate, CustomMoveTo customMoveResult = null)
        {
            TargetPath = targetPath ?? throw new ArgumentNullException(nameof(targetPath));
            RemoteFile = remoteFile;
            UpToDate = upToDate;
            CustomMoveResult = customMoveResult;
        }

        public static async Task<FileInfo> GetTempDownloadFilename()
        {
            var tempPath = Path.Combine(InstallDirectoryHelper.TempDir.FullName, "KKManager_downloads");

        retryCreate:
            try
            {
                Directory.CreateDirectory(tempPath);

            retry:
                var fileName = Path.Combine(tempPath, Path.GetRandomFileName());
                if (File.Exists(fileName))
                    goto retry;

                return new FileInfo(fileName);
            }
            catch (IOException ex)
            {
                if (await ProcessWaiter.CheckForProcessesBlockingKoiDir() != true)
                    throw new IOException($"Failed to create file in directory {tempPath} because of an IO issue - {ex.Message}", ex);

                goto retryCreate;
            }
            catch (SecurityException ex)
            {
                if (MessageBox.Show($"Failed to create file in directory {tempPath} because of a security issue - {ex.Message}\n\nDo you want KK Manager to attempt to fix the issue? Click cancel if you want to abort.",
                        "Could not apply update", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
                {
                    var fixPermissions = ProcessTools.FixPermissions(InstallDirectoryHelper.GameDirectory.FullName);
                    if (fixPermissions == null) throw new IOException($"Failed to create file in directory {tempPath} because of a security issue - {ex.Message}", ex);
                    fixPermissions.WaitForExit();
                    goto retryCreate;
                }
                throw;
            }
        }

        /// <exception cref="IOException">Failed to apply the update.</exception>
        public async Task Update(Progress<double> progressCallback, CancellationToken cancellationToken)
        {
            var inPlace = PathTools.PathsEqual(RemoteFile?.ClientRelativeFileName, TargetPath.FullName);

            var downloadTarget = inPlace ? TargetPath : await GetTempDownloadFilename();
            // Need to store the filename because MoveTo changes it to the new filename
            var downloadFilename = downloadTarget.FullName;

            if (RemoteFile != null)
            {
                Console.WriteLine($"Attempting download of {TargetPath.Name} from source {RemoteFile.Source.Origin}");
                async Task DoDownload() => await RemoteFile.Download(downloadTarget, progressCallback, cancellationToken);
                if (RemoteFile.Source.HandlesRetry)
                    await DoDownload();
                else
                    await RetryHelper.RetryOnExceptionAsync(DoDownload, 2, TimeSpan.FromSeconds(10), cancellationToken);

                downloadTarget.Refresh();
                if (!downloadTarget.Exists || downloadTarget.Length != RemoteFile.ItemSize)
                    throw new IOException($"Failed to download the update file {RemoteFile.Name} - the downloaded file doesn't exist or is corrupted");

                Console.WriteLine($"Downloaded {downloadTarget.Length} bytes successfully");
            }

            if (inPlace)
                return;

            retryDelete:
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(TargetPath.FullName) ?? throw new InvalidOperationException("Invalid path " + TargetPath.FullName));
                try
                {
                    if (TargetPath.Exists)
                    {
                        Console.WriteLine($"Deleting old file {TargetPath.FullName}");
                        // Prevent issues removing readonly files
                        TargetPath.Attributes = FileAttributes.Normal;
                        TargetPath.Delete();
                        // Make sure the file gets deleted before continuing
                        await Task.Delay(200, cancellationToken);
                    }

                    if (RemoteFile != null)
                    {
                        if (CustomMoveResult == null || !CustomMoveResult(downloadTarget, TargetPath, this))
                            downloadTarget.MoveTo(TargetPath.FullName);
                    }
                }
                catch (IOException)
                {
                    if (RemoteFile != null)
                    {
                        await Task.Delay(1000, cancellationToken);
                        if (CustomMoveResult == null || !CustomMoveResult(downloadTarget, TargetPath, this))
                            downloadTarget.Replace(TargetPath.FullName, TargetPath.FullName + ".old", true);
                        await Task.Delay(1000, cancellationToken);
                        File.Delete(TargetPath.FullName + ".old");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            catch (IOException ex)
            {
                if (await ProcessWaiter.CheckForProcessesBlockingKoiDir() != true)
                    throw new IOException($"Failed to apply update {TargetPath.FullName} because of an IO issue - {ex.Message}", ex);

                goto retryDelete;
            }
            catch (SecurityException ex)
            {
                if (MessageBox.Show($"Failed to apply update {TargetPath.FullName} because of a security issue - {ex.Message}\n\nDo you want KK Manager to attempt to fix the issue? Click cancel if you want to abort.", "Could not apply update", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) != DialogResult.OK)
                    throw;

                var fixPermissions = ProcessTools.FixPermissions(InstallDirectoryHelper.GameDirectory.FullName);
                if (fixPermissions == null)
                    throw new IOException($"Failed to create file in directory {TargetPath.FullName} because of a security issue - {ex.Message}", ex);
                fixPermissions.WaitForExit();
                goto retryDelete;
            }
            finally
            {
                try { File.Delete(downloadFilename); }
                catch (SystemException ex) { Console.WriteLine(ex); }
            }
        }

        public FileInfo TargetPath { get; }
        public IRemoteItem RemoteFile { get; }
        public bool UpToDate { get; }
        public CustomMoveTo CustomMoveResult { get; }

        public FileSize GetDownloadSize()
        {
            if (RemoteFile == null) return FileSize.Empty;
            return FileSize.FromBytes(RemoteFile.ItemSize);
        }

        public override string ToString()
        {
            return RemoteFile != null ? RemoteFile + " -> " + TargetPath : "Delete " + TargetPath;
        }
    }
}