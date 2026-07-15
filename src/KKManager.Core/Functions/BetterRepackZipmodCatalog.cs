using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KKManager.Functions
{
    public sealed class BetterRepackZipmodCatalog
    {
        private static readonly Uri Root = new Uri("https://sideload.betterrepack.com/download/AISHS2/");
        private readonly string _catalogPath;
        private string DiscoveredFilesPath => _catalogPath + ".discovered.json";

        public BetterRepackZipmodCatalog(string catalogPath) => _catalogPath = catalogPath;

        public ZipmodCatalog Load()
        {
            try { return File.Exists(_catalogPath) ? JsonConvert.DeserializeObject<ZipmodCatalog>(File.ReadAllText(_catalogPath)) ?? new ZipmodCatalog() : new ZipmodCatalog(); }
            catch (Exception ex) { Console.WriteLine("Failed to load zipmod catalog: " + ex); return new ZipmodCatalog(); }
        }

        public Task<ZipmodCatalog> BuildOrUpdate(CancellationToken cancellationToken, IProgress<CatalogProgress> progress)
        {
            return Task.Run(() =>
            {
                var previous = Load();
                var previousByUrl = previous.Entries.ToDictionary(x => x.Url, StringComparer.OrdinalIgnoreCase);
                // An interrupted build can contain checkpointed GUIDs already. As long as the
                // discovered-URL file exists it is the authoritative incomplete-work marker;
                // only a completed build removes it, after which Update re-crawls listings.
                var files = LoadDiscoveredFiles();
                if (files == null || files.Count == 0)
                {
                    files = DiscoverFiles(cancellationToken, progress);
                    SaveDiscoveredFiles(files);
                }
                else
                {
                    SaveDiscoveredFiles(files);
                    progress?.Report(new CatalogProgress(0, files.Count, "Resuming " + files.Count + " discovered zipmods"));
                }
                var entries = new ConcurrentDictionary<string, ZipmodCatalogEntry>(previousByUrl, StringComparer.OrdinalIgnoreCase);
                var fileUrls = new HashSet<string>(files.Select(x => x.Url), StringComparer.OrdinalIgnoreCase);
                var saveLock = new object();
                var processed = 0;
                // This public archive is sensitive to aggressive range-request bursts. Two fresh
                // connections are substantially more reliable than the previous four workers.
                Parallel.ForEach(files, new ParallelOptions { CancellationToken = cancellationToken, MaxDegreeOfParallelism = 2 }, file =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var current = Interlocked.Increment(ref processed);
                    progress?.Report(new CatalogProgress(current, files.Count, "Reading " + file.FileName));

                    if (previousByUrl.TryGetValue(file.Url, out var old) && string.Equals(old.LastModified, file.LastModified, StringComparison.Ordinal))
                    {
                        return;
                    }

                    try
                    {
                        var guid = ReadManifestGuid(new Uri(file.Url), cancellationToken, out var size);
                        if (!string.IsNullOrWhiteSpace(guid))
                        {
                            file.Guid = guid;
                            file.Size = size;
                            entries[file.Url] = file;
                        }
                    }
                    catch (Exception ex) { Console.WriteLine("Failed to read zipmod manifest " + file.Url + ": " + ex); }
                    finally
                    {
                        // A full scan is large. Persist successful work regularly so the next run
                        // can reuse it after cancellation, a crash, or a transient archive failure.
                        if (current % 25 == 0)
                            lock (saveLock) Save(new ZipmodCatalog { GeneratedUtc = DateTime.UtcNow, Entries = entries.Values.ToList() });
                    }
                });

                var catalog = new ZipmodCatalog { GeneratedUtc = DateTime.UtcNow, Entries = entries.Where(x => fileUrls.Contains(x.Key)).Select(x => x.Value).ToList() };
                Save(catalog);
                try { if (File.Exists(DiscoveredFilesPath)) File.Delete(DiscoveredFilesPath); } catch { }
                progress?.Report(new CatalogProgress(files.Count, files.Count, "Catalog complete: " + entries.Count + " zipmods"));
                return catalog;
            }, cancellationToken);
        }

        public IReadOnlyList<ZipmodCatalogEntry> Find(IEnumerable<string> guids)
        {
            var wanted = new HashSet<string>(guids.Where(x => !string.IsNullOrWhiteSpace(x)), StringComparer.OrdinalIgnoreCase);
            return Load().Entries.Where(x => wanted.Contains(x.Guid))
                .GroupBy(x => x.Guid, StringComparer.OrdinalIgnoreCase)
                .Select(x => x.OrderByDescending(y => y.LastModified).ThenBy(y => y.Url, StringComparer.OrdinalIgnoreCase).First()).ToList();
        }

        private void Save(ZipmodCatalog catalog)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_catalogPath));
            var temp = _catalogPath + ".tmp";
            File.WriteAllText(temp, JsonConvert.SerializeObject(catalog, Formatting.None));
            if (File.Exists(_catalogPath)) File.Replace(temp, _catalogPath, null); else File.Move(temp, _catalogPath);
        }

        private List<ZipmodCatalogEntry> LoadDiscoveredFiles()
        {
            try
            {
                if (File.Exists(DiscoveredFilesPath))
                    return JsonConvert.DeserializeObject<List<ZipmodCatalogEntry>>(File.ReadAllText(DiscoveredFilesPath));

                // One-time recovery for versions that discovered all URLs but failed before
                // they could save the list. The old build wrote each URL to KKManager.log.
                var programDirectory = Directory.GetParent(Path.GetDirectoryName(_catalogPath))?.FullName;
                var logPath = Path.Combine(programDirectory ?? string.Empty, "KKManager.log");
                if (!File.Exists(logPath)) return null;
                var recovered = File.ReadLines(logPath)
                    .Select(x => Regex.Match(x, @"Failed to read zipmod manifest (?<url>https://\S+\.zipmod):"))
                    .Where(x => x.Success)
                    .Select(x => x.Groups["url"].Value)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Select(url => new ZipmodCatalogEntry { Url = url, FileName = Uri.UnescapeDataString(Path.GetFileName(new Uri(url).AbsolutePath)) })
                    .ToList();
                return recovered.Count == 0 ? null : recovered;
            }
            catch (Exception ex) { Console.WriteLine("Failed to load discovered zipmod URLs: " + ex); return null; }
        }

        private void SaveDiscoveredFiles(IEnumerable<ZipmodCatalogEntry> files)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DiscoveredFilesPath));
            var temp = DiscoveredFilesPath + ".tmp";
            File.WriteAllText(temp, JsonConvert.SerializeObject(files, Formatting.None));
            if (File.Exists(DiscoveredFilesPath)) File.Replace(temp, DiscoveredFilesPath, null); else File.Move(temp, DiscoveredFilesPath);
        }

        private static List<ZipmodCatalogEntry> DiscoverFiles(CancellationToken token, IProgress<CatalogProgress> progress)
        {
            var files = new List<ZipmodCatalogEntry>();
            var pending = new Queue<Uri>();
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            pending.Enqueue(Root);
            while (pending.Count > 0)
            {
                token.ThrowIfCancellationRequested();
                var directory = pending.Dequeue();
                if (!visited.Add(directory.AbsoluteUri)) continue;
                progress?.Report(new CatalogProgress(files.Count, 0, "Scanning " + directory.AbsolutePath));
                string html;
                try { html = DownloadText(directory, token); }
                catch (Exception ex)
                {
                    // Without the root listing there is nothing safe to catalog.  Do not create an
                    // empty catalog and make it look like a successful build when the archive is down.
                    if (directory == Root)
                        throw new IOException("Could not connect to the BetterRepack AISHS2 archive. The catalog was not changed. " +
                                              "Open " + Root + " in a browser and try again when it is reachable.", ex);

                    Console.WriteLine("Failed to scan catalog directory " + directory + ": " + ex.Message);
                    continue;
                }
                foreach (Match match in Regex.Matches(html, "href=\\\"(?<href>[^\\\"]+)\\\"(?<tail>.{0,100})", RegexOptions.IgnoreCase | RegexOptions.Singleline))
                {
                    var href = WebUtility.HtmlDecode(match.Groups["href"].Value);
                    if (href == "../" || href.StartsWith("?")) continue;
                    if (!Uri.TryCreate(directory, href, out var link) || !IsAllowed(link)) continue;
                    if (link.AbsolutePath.EndsWith("/")) pending.Enqueue(link);
                    else if (link.AbsolutePath.EndsWith(".zipmod", StringComparison.OrdinalIgnoreCase))
                    {
                        var modified = Regex.Match(match.Groups["tail"].Value, @"(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2})").Groups[1].Value;
                        files.Add(new ZipmodCatalogEntry { Url = link.AbsoluteUri, FileName = Uri.UnescapeDataString(Path.GetFileName(link.AbsolutePath)), LastModified = modified });
                    }
                }
            }
            return files.GroupBy(x => x.Url, StringComparer.OrdinalIgnoreCase).Select(x => x.First()).ToList();
        }

        private static bool IsAllowed(Uri uri) => uri.Scheme == Uri.UriSchemeHttps && uri.Host.Equals(Root.Host, StringComparison.OrdinalIgnoreCase) && uri.AbsolutePath.StartsWith(Root.AbsolutePath, StringComparison.OrdinalIgnoreCase);

        public static string ReadManifestGuid(Uri uri, CancellationToken token, out long size)
        {
            // .NET Framework's HttpWebRequest only supports its AddRange(from, to) API,
            // so get the archive length first and then request the ZIP tail explicitly.
            DownloadRange(uri, 0, 0, token, out var totalLength);
            var tailStart = Math.Max(0, totalLength - 65557);
            var tail = DownloadRange(uri, tailStart, totalLength - 1, token, out _);
            size = totalLength;
            var eocd = FindSignature(tail, 0x06054b50);
            if (eocd < 0) throw new InvalidDataException("ZIP end record was not found");
            var centralSize = ReadUInt32(tail, eocd + 12);
            var centralOffset = ReadUInt32(tail, eocd + 16);
            var central = DownloadRange(uri, centralOffset, centralOffset + centralSize - 1, token, out _);
            var position = 0;
            while (position + 46 <= central.Length && ReadUInt32(central, position) == 0x02014b50)
            {
                var method = ReadUInt16(central, position + 10);
                var compressedSize = ReadUInt32(central, position + 20);
                var nameLength = ReadUInt16(central, position + 28);
                var extraLength = ReadUInt16(central, position + 30);
                var commentLength = ReadUInt16(central, position + 32);
                var localOffset = ReadUInt32(central, position + 42);
                var name = Encoding.UTF8.GetString(central, position + 46, nameLength);
                if (name.EndsWith("manifest.xml", StringComparison.OrdinalIgnoreCase))
                {
                    var local = DownloadRange(uri, localOffset, localOffset + 30 + nameLength + extraLength + compressedSize + 64, token, out _);
                    if (ReadUInt32(local, 0) != 0x04034b50) throw new InvalidDataException("ZIP local header was not found");
                    var localNameLength = ReadUInt16(local, 26);
                    var localExtraLength = ReadUInt16(local, 28);
                    var payloadOffset = 30 + localNameLength + localExtraLength;
                    using var input = new MemoryStream(local, payloadOffset, (int)compressedSize, false);
                    using var decoded = method == 8 ? (Stream)new DeflateStream(input, CompressionMode.Decompress) : input;
                    using var reader = new StreamReader(decoded, Encoding.UTF8, true);
                    var xml = reader.ReadToEnd();
                    var guid = Regex.Match(xml, @"<guid>\s*(?<guid>[^<]+)\s*</guid>", RegexOptions.IgnoreCase).Groups["guid"].Value.Trim();
                    return guid;
                }
                position += 46 + nameLength + extraLength + commentLength;
            }
            return null;
        }

        private static string DownloadText(Uri uri, CancellationToken token) => Encoding.UTF8.GetString(DownloadRange(uri, null, null, token, out _));
        private static byte[] DownloadRange(Uri uri, long? from, long? to, CancellationToken token, out long totalLength)
        {
            // KKManager targets .NET Framework 4.7.2; explicitly enable the TLS version required by the HTTPS archive.
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            Exception last = null;
            for (var attempt = 0; attempt < 3; attempt++)
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(uri);
                    ConfigureRequest(request);
                    if (from.HasValue) request.AddRange(from.Value, to ?? from.Value);
                    using var response = (HttpWebResponse)request.GetResponse();
                    using var stream = response.GetResponseStream();
                    using var output = new MemoryStream();
                    stream.CopyTo(output);
                    var bytes = output.ToArray();
                    var contentRange = response.Headers["Content-Range"];
                    var separator = contentRange?.LastIndexOf('/') ?? -1;
                    totalLength = separator >= 0 && long.TryParse(contentRange.Substring(separator + 1), out var parsedLength) ? parsedLength : bytes.LongLength;
                    token.ThrowIfCancellationRequested();
                    return bytes;
                }
                catch (WebException ex) when (attempt < 2) { last = ex; Thread.Sleep(1000 * (attempt + 1)); }
            }
            totalLength = 0;
            throw last ?? new IOException("Failed to download " + uri);
        }
        private static int FindSignature(byte[] bytes, uint signature) { for (var i = bytes.Length - 4; i >= 0; i--) if (ReadUInt32(bytes, i) == signature) return i; return -1; }
        private static ushort ReadUInt16(byte[] bytes, int offset) => BitConverter.ToUInt16(bytes, offset);
        private static uint ReadUInt32(byte[] bytes, int offset) => BitConverter.ToUInt32(bytes, offset);
        private static void ConfigureRequest(HttpWebRequest http)
        {
            // The archive honours ranges over HTTP/1.1, but its connection handling is
            // unreliable with persistent/default-compressed requests.
            http.Timeout = 30000;
            http.ReadWriteTimeout = 30000;
            http.KeepAlive = false;
            http.AutomaticDecompression = DecompressionMethods.None;
            http.UserAgent = "KKManager BetterRepack zipmod catalog";
        }
    }
    public sealed class ZipmodCatalog { public DateTime GeneratedUtc { get; set; } = DateTime.MinValue; public List<ZipmodCatalogEntry> Entries { get; set; } = new List<ZipmodCatalogEntry>(); }
    public sealed class ZipmodCatalogEntry { public string Guid { get; set; } public string Url { get; set; } public string FileName { get; set; } public string LastModified { get; set; } public long Size { get; set; } }
    public sealed class CatalogProgress { public CatalogProgress(int processed, int total, string status) { Processed = processed; Total = total; Status = status; } public int Processed { get; } public int Total { get; } public string Status { get; } }
}
