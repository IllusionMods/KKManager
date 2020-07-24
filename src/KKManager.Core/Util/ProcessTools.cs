using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
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
            // false for cancel
            if (ProcessWaiter.ProcessWaiter.CheckForProcessesBlockingKoiDir().Result == false)
                return null;

            var batContents = $@"
title Fixing permissions... 
rem Get the localized version of Y/N to pass to takeown to make this work in different locales
for /f ""tokens=1,2 delims=[,]"" %%a in ('""choice <nul 2>nul""') do set ""yes=%%a"" & set ""no=%%b""
echo Press %yes% for yes and %no% for no
set target={ path.Trim(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar, ' ') }
echo off
cls
echo Taking ownership of %target% ...
rem First find is to filter out success messages, second findstr is to filter out empty lines
takeown /F ""%target%"" /R /SKIPSL /D %yes% | find /V ""SUCCESS: The file (or folder):"" | findstr /r /v ""^$""
echo.
echo Fixing access rights ...
icacls ""%target%"" /grant *S-1-1-0:(OI)(CI)F /T /C /L /Q
echo.
echo Finished! If the process failed, reboot your PC and try again.
pause";
            var batPath = Path.Combine(Path.GetTempPath(), "kkmanager_fixperms.bat");
            File.WriteAllText(batPath, batContents);

            return SafeStartProcess(new ProcessStartInfo("cmd", $"/C \"{batPath}\""), true);
        }

        /// <summary>
        ///     Get IDs of all child processes
        /// </summary>
        public static IEnumerable<int> GetChildProcesses(int pid)
        {
            var searcher = new ManagementObjectSearcher
                ("Select * From Win32_Process Where ParentProcessID=" + pid);

            using (var moc = searcher.Get())
            {
                return moc.Cast<ManagementObject>().Select(mo => Convert.ToInt32(mo["ProcessID"])).ToList();
            }
        }
    }
}
