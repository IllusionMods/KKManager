using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Ionic.Zip;
using KKManager.SB3UGS;
using KKManager.Util;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Writers.Zip;
using CompressionLevel = Ionic.Zlib.CompressionLevel;

namespace KKManager.ModpackTool;

public static class ZipmodProcessor
{
    private static ModpackToolConfiguration Configuration => ModpackToolConfiguration.Instance;

    private static readonly ConcurrentQueue<ZipmodEntry> _ProcessingQueue = new();

    private static volatile int _tasksRunning;

    public static void ProcessZipmods(List<ZipmodEntry> targets)
    {
        lock (_ProcessingQueue)
        {
            foreach (var zipmodEntry in targets)
            {
                zipmodEntry.Status = ZipmodEntry.ZipmodEntryStatus.Processing;
                _ProcessingQueue.Enqueue(zipmodEntry);
            }

            StartProcessingThreadIfNeeded();
        }
    }

    private static void StartProcessingThreadIfNeeded()
    {
        var maxToSpawn = Math.Min(_ProcessingQueue.Count, 4);
        while (!_ProcessingQueue.IsEmpty && _tasksRunning < maxToSpawn)
        {
            Interlocked.Increment(ref _tasksRunning);
            Task.Run(ProcessingThread).ContinueWith(_ => Interlocked.Decrement(ref _tasksRunning));
        }
    }

    private static async Task ProcessingThread()
    {
        while (_ProcessingQueue.TryDequeue(out var toProcess))
        {
            // Need to keep a copy just in case toProcess gets changed while some lambdas are still running
            var zipmodEntry = toProcess;
            try
            {
                var tempDir = zipmodEntry.GetTempDir();
                var outPath = zipmodEntry.GetTempOutputFilePath();

                // ------ Clean up
                try
                {
                    if (Directory.Exists(tempDir))
                        Directory.Delete(tempDir, true);
                }
                catch
                {
                    await Task.Delay(500);
                    if (Directory.Exists(tempDir))
                        Directory.Delete(tempDir, true);
                }
                Directory.CreateDirectory(Path.GetDirectoryName(outPath) ?? throw new InvalidOperationException(outPath + " got NULL from Path.GetDirectoryName"));
                try
                {
                    File.Delete(outPath);
                }
                catch
                {
                    await Task.Delay(500);
                    File.Delete(outPath);
                }

                // ------ Extract zipmod to temp
                using (var zf = ArchiveFactory.Open(zipmodEntry.FullPath.FullName))
                {
                    zf.ExtractArchiveToDirectory(tempDir);
                }

                // todo might work better but might also lock up if there's a ton of files
                //await Task.WhenAll(Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories).Select(async f =>
                //{
                //    try
                //    {
                //        await ProcessFile(zipmodEntry, new FileInfo(f));
                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine($"Failed to process file [{f}], the resulting zipmod might have issues. Error: {e}");
                //    }
                //}));

                Parallel.ForEach(Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories), s =>
                {
                    try
                    {
                        ProcessFile(zipmodEntry, new FileInfo(s)).Wait();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed to process file [{s}], the resulting zipmod might have issues. Error: {e}");
                    }
                });

                // ------ Write new manifest
                var manifestPath = Path.Combine(tempDir, "manifest.xml");

                // todo remove/keep somewhere else for reoport
                //File.Move(manifestPath, manifestPath + ".old");
                await new FileInfo(manifestPath).SafeDelete();

                var manifestWorkCopy = new XDocument(zipmodEntry.Info.Manifest.ManifestDocument);

                // Clean up manifest comments
                var archiveComment = $"Generated with KKManager v{Assembly.GetExecutingAssembly().GetName().Version} (ModpackTool)";
                manifestWorkCopy.DescendantNodes().OfType<XComment>().Remove();
                manifestWorkCopy.AddFirst(new XComment(archiveComment));
                using (var writer = File.OpenWrite(manifestPath))
                    manifestWorkCopy.Save(writer, SaveOptions.OmitDuplicateNamespaces);

                // ------ Recompress the archive
                //using (var zf = ZipArchive.Create())
                //using (var writeStream = File.OpenWrite(outPath))
                //{
                //    zf.DeflateCompressionLevel = CompressionLevel.None;
                //    zf.AddAllFromDirectory(tempDir);
                //    zf.SaveTo(writeStream, new ZipWriterOptions(CompressionType.None)
                //    {
                //        //ArchiveEncoding = new ArchiveEncoding(Encoding.UTF8, Encoding.UTF8),
                //        //ArchiveComment = archiveComment
                //    });
                //}

                using (var writeStream = File.OpenWrite(outPath))
                using (var zf = new Ionic.Zip.ZipFile())
                {
                    zf.CompressionMethod = CompressionMethod.None;
                    zf.CompressionLevel = CompressionLevel.None;
                    zf.Comment = archiveComment;

                    zf.AddDirectory(tempDir, "");
                    zf.Save(writeStream);
                }







                // ------ Clean up
                await new DirectoryInfo(tempDir).SafeDelete();

                // ------ Success
                zipmodEntry.Status = ZipmodEntry.ZipmodEntryStatus.NeedsVerify;

                Console.WriteLine("Successfully finished processing " + zipmodEntry.OriginalFilename);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to process zipmod {zipmodEntry.OriginalFilename} with error: {e}");

                zipmodEntry.Status = ZipmodEntry.ZipmodEntryStatus.FAIL;

                if (!string.IsNullOrEmpty(zipmodEntry.Notes))
                    zipmodEntry.Notes += " ";
                zipmodEntry.Notes += "Processing failed: " + e.Message;
            }
        }
    }

    private static async Task ProcessFile(ZipmodEntry owner, FileInfo fileInfo)
    {
        var path = fileInfo.FullName;

        bool IsModFile(FileInfo file)
        {
            var tempDir = owner.GetTempDir();
            return file.FullName.StartsWith(Path.Combine(tempDir, "manifest.xml"), StringComparison.OrdinalIgnoreCase) ||
                   file.FullName.StartsWith(Path.Combine(tempDir, "abdata"), StringComparison.OrdinalIgnoreCase);
        }

        if (!IsModFile(fileInfo))
        {
            switch (fileInfo.Extension.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    //todo fileInfo.CopyTo(Path.Join(dirs.LooseImages, fileInfo.Name), true);
                    Console.WriteLine("Deleting unnecessary file: " + path);
                    await fileInfo.SafeDelete();
                    return;
                case ".png":
                    //todo
                    //if (await CardUtilities.ContainsDataAfterIEndAsync(filename))
                    //{
                    //    fileInfo.CopyTo(Path.Join(dirs.LooseCards, fileInfo.Name), true);
                    //}
                    //else
                    //{
                    //    fileInfo.CopyTo(Path.Join(dirs.LooseImages, fileInfo.Name), true);
                    //}
                    Console.WriteLine("Deleting unnecessary file: " + path);
                    await fileInfo.SafeDelete();
                    return;
                default:
                    return;
            }
        }
        else
        {
            switch (fileInfo.Extension.ToLower())
            {
                case ".png":
                    if (fileInfo.Name.EndsWith("-crushed.png"))
                    {
                        // Most likely another thread is crushing? Shouldn't actually happen?
                        break;
                    }
                    // todo
                    //if (await CardUtilities.ContainsDataAfterIEndAsync(filename))
                    //{
                    //    // dealing with a card
                    //    fileInfo.CopyTo(Path.Join(dirs.LooseCards, fileInfo.Name), true);
                    //    fileInfo.Delete();
                    //    await _sessionService.CommitResultAsync(new SessionResult(zipmod, filename, SessionResultType.ImageDeleted));
                    //}
                    //else
                    if (Configuration.CompressPNGs)
                    {
                        Console.WriteLine("Compressing PNG: " + path);

                        await CompressImageAsync(fileInfo);
                        //if (didCompressImage)
                        //{
                        //    _logger.LogInformation("Image compression of {filename} took {time}ms", filename, compressImageTime.TotalMilliseconds);
                        //    await _sessionService.CommitResultAsync(new SessionResult(zipmod, filename, SessionResultType.ImageCompressed));
                        //}
                        //else
                        //{
                        //    // an error occured
                        //    fileInfo.CopyTo(Path.Join(dirs.LooseImages, fileInfo.Name), true);
                        //    await _sessionService.CommitResultAsync(new SessionResult(zipmod, filename, SessionResultType.NoChange));
                        //}
                    }
                    // if compressed, it'll create a {filename}-orig.png, with the new file being compressed

                    break;
                case ".unity3d":
                    if (owner.Recompress)
                    {
                        try
                        {
                            var sw = Stopwatch.StartNew();
                            var origSize = fileInfo.Length;
                            SB3UGS_Utils.CompressBundle(fileInfo.FullName, Configuration.RandomizeCABs);
                            fileInfo.Refresh();
                            var sizeSaved = origSize - fileInfo.Length;
                            Console.WriteLine($"Recompressed bundle {(Configuration.RandomizeCABs ? "and randomized CAB " : "")}in {sw.ElapsedMilliseconds}ms ({FileSize.FromBytes(sizeSaved)} saved) [{fileInfo.FullName}]");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to compress file {fileInfo.FullName} - {ex.Message}");
                        }
                    }

                    //if (repository.Configuration.RandomizeCab)
                    //{
                    //    _logger.LogInformation("Randomizing CAB for {file}", filename);
                    //    var didRandomizeCab = false;
                    //    var randomizeCabDuration = await TimingUtilities.TimeAsync(async () =>
                    //    {
                    //        didRandomizeCab = await _assetService.RandomizeCabAsync(repository.Configuration, filename);
                    //
                    //    });
                    //    await _sessionService.CommitResultAsync(new SessionResult(zipmod, filename, didRandomizeCab ? SessionResultType.ResourceCabRandomized : SessionResultType.NoChange));
                    //    if (didRandomizeCab)
                    //    {
                    //        _logger.LogInformation("CAB randomization of {file} took {time}ms", filename, randomizeCabDuration.TotalMilliseconds);
                    //    }
                    //
                    //}
                    //if (repository.Configuration.SkipCompression)
                    //{
                    //    _logger.LogInformation("Compression skipped, copying");
                    //    await _sessionService.CommitResultAsync(new SessionResult(zipmod, filename, SessionResultType.ResourceCopied));
                    //}
                    //else if (repository.Configuration.SkipCharaMods && hasCharaMods)
                    //{
                    //    _logger.LogInformation("Compression skipped for chara mods");
                    //    await _sessionService.CommitResultAsync(new SessionResult(zipmod, filename, SessionResultType.ResourceCopied));
                    //}
                    //else
                    //{
                    //    _logger.LogInformation("Compressing unity3d file");
                    //    var didCompressResx = false;
                    //    var compressionDuration = await TimingUtilities.TimeAsync(async () =>
                    //    {
                    //        didCompressResx = await _assetService.CompressUnityResxAsync(repository.Configuration, filename);
                    //    });
                    //    await _sessionService.CommitResultAsync(new SessionResult(zipmod, filename, didCompressResx ? SessionResultType.ResourceCompressed : SessionResultType.NoChange));
                    //    if (didCompressResx)
                    //    {
                    //        _logger.LogInformation("Compression of {file} took {time}ms", filename, compressionDuration.TotalMilliseconds);
                    //    }
                    //
                    //}
                    break;
                case ".csv":
                case ".xml":
                case ".txt":
                    //await _sessionService.CommitResultAsync(new SessionResult(zipmod, filename, SessionResultType.ResourceSkipped));
                    break;
                case ".jpg":
                case ".jpeg":
                    break;
                case ".tmp":
                    // this file is being worked on, skip //todo not really?
                    break;
                default:
                    //_logger.LogInformation("Invalid file {file}, deleting", filename);
                    //await _sessionService.CommitResultAsync(new SessionResult(zipmod, filename, SessionResultType.ResourceDeleted));
                    Console.WriteLine("Removing invalid/unknown file: " + path);
                    await fileInfo.SafeDelete();
                    break;
            }
        }
    }

    public static async Task<bool> CompressImageAsync(FileInfo originalFileInfo)
    {
        var sw = Stopwatch.StartNew();

        var originalFilename = originalFileInfo.FullName;
        Debug.Assert(originalFileInfo.Directory != null, "originalFileInfo.Directory != null");
        var crushedFilename = Path.Combine(originalFileInfo.Directory.FullName, $"{Path.GetFileNameWithoutExtension(originalFileInfo.Name)}-crushed.png"); //todo better name?
        var crushedFileInfo = new FileInfo(crushedFilename);
        try
        {
            // compress
            var process = BuildPngCrushProcess(originalFilename, crushedFilename);

            process.Exited += (_, _) =>
            {
                if (process.ExitCode != 0)
                {
                    throw new TargetException($"pngcrush exited with non-zero exit code ({process.ExitCode})");
                }
            };
            process.Start();
            await process.WaitForExitAsync();

            var crushedSize = crushedFileInfo.Length;
            var origsize = originalFileInfo.Length;
            if (crushedSize == 0)
            {
                throw new TargetException($"Compressed PNG size is 0 bytes somehow, keeping original file at {FileSize.FromBytes(origsize)}");
            }
            else if (crushedSize < origsize)
            {
                // 1. delete original file
                await originalFileInfo.SafeDelete();
                // 2. copy over crushed file
                File.Move(crushedFilename, originalFilename);
                // 3. delete original crushed file
                // File.Delete(crushedFilename); not needed, done in finally
                Console.WriteLine($"Compressed PNG from {FileSize.FromBytes(origsize)} to {FileSize.FromBytes(crushedSize)} in {sw.ElapsedMilliseconds}ms. [{originalFilename}]");
            }
            else
            {
                Console.WriteLine($"Compressed PNG is larger than original ({FileSize.FromBytes(origsize)} -> {FileSize.FromBytes(crushedSize)}) in {sw.ElapsedMilliseconds}ms, keeping original. [{originalFilename}]");
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to PNG compress [{originalFilename}] in {sw.ElapsedMilliseconds}ms, original file will be used. Error: {(ex is TargetException ? ex.Message : ex)}");
            return false;
        }
        finally
        {
            // Always clean up the temp file
            await crushedFileInfo.SafeDelete();
        }
    }

    private static Process BuildPngCrushProcess(string inputFilename, string outputFilename)
    {
        var path = Directory.GetFiles(ModpackToolWindow.ModpackToolRootDir, "pngcrush*.exe", SearchOption.TopDirectoryOnly).OrderByDescending(x => x).FirstOrDefault();
        if (path == null) throw new FileNotFoundException("Could not find any pngcrunch.exe inside of " + ModpackToolWindow.ModpackToolRootDir);
        return new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = path,
                CreateNoWindow = true,
                UseShellExecute = false,
                // put extra quotes around just to make sure
                Arguments = string.Join(" ", "-reduce", "-brute", $"\"{inputFilename}\"", $"\"{outputFilename}\""),
            },
            EnableRaisingEvents = true,
            //PriorityBoostEnabled = true,
            //PriorityClass = ProcessPriorityClass.RealTime,
        };
    }
}
