using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;

namespace KKManager.Util
{
    public static class ProcessTools
    {
        public static Process SafeStartProcess(ProcessStartInfo startInfo, bool elevated = false)
        {
            try
            {
                if (elevated && !IsAdministrator())
                {
                    startInfo.Verb = "runas";
                    startInfo.UseShellExecute = true;

                    if (startInfo.RedirectStandardInput ||
                        startInfo.RedirectStandardError ||
                        startInfo.RedirectStandardOutput)
                        throw new Exception("Can't redirect standard in/out/error when elevating");
                }

                return Process.Start(startInfo);
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Failed to start application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static Process SafeStartProcess(string filename, bool elevated = false)
        {
            var startInfo = new ProcessStartInfo(filename);

            if (File.Exists(filename))
                startInfo.WorkingDirectory = Path.GetDirectoryName(filename) ?? filename;

            startInfo.UseShellExecute = true;

            return SafeStartProcess(startInfo, elevated);
        }

        private static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static Process FixPermissions(string path)
        {
            path = path.Trim(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar, ' '); 
            return SafeStartProcess(new ProcessStartInfo("cmd", $"/C echo off & cls & title Fixing permissions... & takeown /f \"{ path }\" /r /SKIPSL /d y & icacls \"{ path }\" /grant everyone:F /t /c /l & pause"), true);
        }
    }
}
