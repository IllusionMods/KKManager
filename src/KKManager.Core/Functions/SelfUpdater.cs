﻿using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Properties;
using KKManager.Util;

namespace KKManager.Functions
{
    public static class SelfUpdater
    {
        private static readonly string _LatestReleaseUrl = "https://github.com/IllusionMods/KKManager/releases/latest";

        public static async Task<Version> CheckLatestVersion()
        {
            // Should result in something like "https://github.com/IllusionMods/KKManager/releases/tag/v0.14.1"
            var url = await GetFinalRedirect(_LatestReleaseUrl);
            var i = url.LastIndexOf('/');
            var tag = url.Substring(i).TrimStart('/', 'v');
            return new Version(tag);
        }

        /// <summary>
        /// Returns null if failed to look for updates, else returns if there is a newer version available
        /// </summary>
        public static async Task<bool?> IsUpdateAvailable()
        {
            try
            {
                var latestVersion = await CheckLatestVersion();
                if (latestVersion > new Version(Constants.Version.TrimEnd('*', '.')))
                {
                    Console.WriteLine("[SelfUpdater] A new KKManager version is available: " + latestVersion);
                    return true;
                }
                else
                {
                    Console.WriteLine("[SelfUpdater] The current version of KKManager is the latest");
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[SelfUpdater] Failed to check for new KKManager versions: " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Returns null if no updates were found, else returns if user clicked yes
        /// </summary>
        public static async Task<bool?> CheckForUpdatesAndShowDialog()
        {
            var isUpdateAvailable = await IsUpdateAvailable();
            if (isUpdateAvailable != true) return null;

            if (MessageBox.Show(Resources.SelfUpdater_NewVersionMessage, Resources.SelfUpdater_NewVersionTitle,
                MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                return ProcessTools.SafeStartProcess(_LatestReleaseUrl) != null;
            else
                return false;
        }

        // https://stackoverflow.com/a/28424940
        private static async Task<string> GetFinalRedirect(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return url;

            int maxRedirCount = 8; // prevent infinite loops
            string newUrl = url;
            do
            {
                WebResponse resp = null;
                try
                {
                    var req = (HttpWebRequest)WebRequest.Create(url);
                    req.Method = "HEAD";
                    req.AllowAutoRedirect = false;
                    try
                    {
                        var httpresp = (HttpWebResponse)await req.GetResponseAsync();

                        switch (httpresp.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                return newUrl;
                            case HttpStatusCode.Redirect:
                            case HttpStatusCode.MovedPermanently:
                            case HttpStatusCode.RedirectKeepVerb:
                            case HttpStatusCode.RedirectMethod:
                                // Handle redirects below
                                break;
                            default:
                                return newUrl;
                        }

                        resp = httpresp;
                    }
                    catch (WebException ex)
                    {
                        // Handle redirects below. Needed for .NET Core because for some reason it throws on success
                        if (ex.Message.Contains("302"))
                            resp = ex.Response;
                        else throw;
                    }

                    newUrl = resp.Headers["Location"];
                    if (newUrl == null)
                        return url;

                    if (newUrl.IndexOf("://", StringComparison.Ordinal) == -1)
                    {
                        // Doesn't have a URL Schema, meaning it's a relative or absolute URL
                        Uri u = new Uri(new Uri(url), newUrl);
                        newUrl = u.ToString();
                    }

                    url = newUrl;
                }
                catch (WebException)
                {
                    // Return the last known good URL
                    return newUrl;
                }
                catch (Exception)
                {
                    return null;
                }
                finally
                {
                    if (resp != null)
                        resp.Close();
                }
            } while (maxRedirCount-- > 0);

            return newUrl;
        }
    }
}
