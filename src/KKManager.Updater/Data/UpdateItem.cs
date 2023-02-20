using System;
using System.IO;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Functions;
using KKManager.Updater.Properties;
using KKManager.Updater.Sources;
using KKManager.Updater.Utils;
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

        public static string GetTempDownloadDirectory()
        {
            return Path.Combine(InstallDirectoryHelper.TempDir.FullName, "KKManager_downloads");
        }

        public static async Task<FileInfo> GetTempDownloadFilename()
        {
            var tempPath = GetTempDownloadDirectory();

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
                if (await ProcessWaiter.CheckForProcessesBlockingKoiDir().ConfigureAwait(false) != true)
                    throw new IOException($"Failed to create file in directory {tempPath} because of an IO issue - {ex.Message}", ex);

                goto retryCreate;
            }
            catch (SecurityException ex)
            {
                if (MessageBox.Show(string.Format(Resources.SecurityExceptionRetry_Message, tempPath, ex.Message),
                        Resources.SecurityExceptionRetry_Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
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

            var downloadTarget = inPlace ? TargetPath : await GetTempDownloadFilename().ConfigureAwait(false);
            // Need to store the filename because MoveTo changes it to the new filename
            var downloadFilename = downloadTarget.FullName;

            if (RemoteFile != null)
            {
                Console.WriteLine($"Attempting download of [{TargetPath.Name}] ({GetDownloadSize()}) from source {RemoteFile.Source.Origin}");
                async Task DoDownload() => await RemoteFile.Download(downloadTarget, progressCallback, cancellationToken).ConfigureAwait(false);
                if (RemoteFile.Source.HandlesRetry)
                    await DoDownload().ConfigureAwait(false);
                else
                    await RetryHelper.RetryOnExceptionAsync(DoDownload, 2, TimeSpan.FromSeconds(10), cancellationToken).ConfigureAwait(false);

                downloadTarget.Refresh();
                if (CustomMoveResult == null && (!downloadTarget.Exists || downloadTarget.Length != RemoteFile.ItemSize)) //bug a better way than CustomMoveResult is needed 
                    throw new IOException($"Failed to download the update file {RemoteFile.Name} - the downloaded file doesn't exist or is corrupted");
                
                // todo make this show size of the actual downloaded file? needs changes to torrent update items to make it work
                Console.WriteLine($"Downloaded [{TargetPath.Name}] successfully ({RemoteFile.GetFancyItemSize()})");
            }

            if (inPlace)
                return;

            retryDelete:
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(TargetPath.FullName) ?? throw new InvalidOperationException("Invalid path " + TargetPath.FullName));
                try
                {
                    if (CustomMoveResult == null || !CustomMoveResult(downloadTarget, TargetPath, this))
                    {
                        if (TargetPath.Exists)
                        {
                            Console.WriteLine($"Deleting old file {TargetPath.FullName}");
                            // Prevent issues removing readonly files
                            TargetPath.Attributes = FileAttributes.Normal;
                            TargetPath.Delete();
                            // Make sure the file gets deleted before continuing
                            await Task.Delay(200, cancellationToken).ConfigureAwait(false);
                        }

                        if (RemoteFile != null)
                        {
                            downloadTarget.MoveTo(TargetPath.FullName);
                        }
                    }
                }
                catch (IOException)
                {
                    if (RemoteFile != null)
                    {
                        await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
                        if (CustomMoveResult == null || !CustomMoveResult(downloadTarget, TargetPath, this))
                        {
                            downloadTarget.Replace(TargetPath.FullName, TargetPath.FullName + ".old", true);
                            await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
                            File.Delete(TargetPath.FullName + ".old");
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            catch (IOException ex)
            {
                if (await ProcessWaiter.CheckForProcessesBlockingKoiDir().ConfigureAwait(false) != true)
                    throw new IOException($"Failed to apply update {TargetPath.FullName} because of an IO issue - {ex.Message}", ex);

                goto retryDelete;
            }
            catch (SecurityException ex)
            {
                if (MessageBox.Show(string.Format(Resources.SecurityExceptionRetry_Message, TargetPath.FullName, ex.Message),
                                    Resources.SecurityExceptionRetry_Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Error) != DialogResult.OK)
                    throw;

                var fixPermissions = ProcessTools.FixPermissions(InstallDirectoryHelper.GameDirectory.FullName);
                if (fixPermissions == null)
                    throw new IOException($"Failed to create file in directory {TargetPath.FullName} because of a security issue - {ex.Message}", ex);
                fixPermissions.WaitForExit();
                goto retryDelete;
            }
            finally
            {
                // this clashes with torrent downloads
                //try { File.Delete(downloadFilename); }
                //catch (SystemException ex) { Console.WriteLine(ex); }
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