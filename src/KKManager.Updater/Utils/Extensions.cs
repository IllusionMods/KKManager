using System;
using System.Diagnostics;
using KKManager.Updater.Data;
using KKManager.Util;
using MonoTorrent.Client;

namespace KKManager.Updater.Utils
{
    public static class Extensions
    {
        public static FileSize GetFancyItemSize(this IRemoteItem item) => FileSize.FromBytes(item.ItemSize);
        

        [Conditional("DEBUG")]
        public static void AddDebugLogging(this ClientEngine clientEngine)
        {
            foreach (var torrentManager in clientEngine.Torrents)
            {
                AddDebugLogging(torrentManager);
            }

            clientEngine.ConnectionManager.BanPeer += (sender, eventArgs) =>
            {
                if (eventArgs.BanPeer)
                    Console.WriteLine($"BanPeer BanPeer={eventArgs.BanPeer} Peer={eventArgs.Peer.ConnectionUri}");
            };
        }
        [Conditional("DEBUG")]
        public static void AddDebugLogging(this TorrentManager torrentManager)
        {
#if DEBUG
            torrentManager.PieceHashed += (sender, eventArgs) =>
            {
                if (!eventArgs.HashPassed)
                    Console.WriteLine($"[{eventArgs.TorrentManager.Name}] PieceHashed FAILED PieceIndex={eventArgs.PieceIndex} Progress={eventArgs.Progress}");
            };
#endif
            torrentManager.ConnectionAttemptFailed += (sender, eventArgs) => Console.WriteLine($"[{eventArgs.TorrentManager.Name}] ConnectionAttemptFailed Peer={eventArgs.Peer.ConnectionUri} Reason={eventArgs.Reason}");
            torrentManager.PeerDisconnected += (sender, eventArgs) => Console.WriteLine($"[{eventArgs.TorrentManager.Name}] PeerDisconnected Peer={eventArgs.Peer.Uri}");
            torrentManager.PeerConnected += (sender, eventArgs) => Console.WriteLine($"[{eventArgs.TorrentManager.Name}] PeerConnected Peer={eventArgs.Peer.Uri}");
            torrentManager.PeersFound += (sender, eventArgs) => Console.WriteLine($"[{eventArgs.TorrentManager.Name}] PeersFound ExistingPeers={eventArgs.ExistingPeers} NewPeers={eventArgs.NewPeers}");
            torrentManager.TrackerManager.AnnounceComplete += (sender, eventArgs) => Console.WriteLine($"[{torrentManager.Name}] AnnounceComplete Successful={eventArgs.Successful} Tracker={eventArgs.Tracker.Uri} PeersCount={eventArgs.Peers.Count}");
            torrentManager.TrackerManager.ScrapeComplete += (sender, eventArgs) => Console.WriteLine($"[{torrentManager.Name}] ScrapeComplete Successful={eventArgs.Successful} Tracker={eventArgs.Tracker.Uri}");
        }
    }
}
