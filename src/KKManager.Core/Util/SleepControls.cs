using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KKManager.Util
{
    public static class SleepControls
    {
        public static bool PreventSleepOrShutdown(IntPtr windowHandle, string blockReason)
        {
            SetThreadExecutionState(ExecutionState.EsContinuous | ExecutionState.EsSystemRequired);
            return ShutdownBlockReasonCreate(windowHandle, blockReason);
        }

        public static bool AllowSleepOrShutdown(IntPtr windowHandle)
        {
            SetThreadExecutionState(ExecutionState.EsContinuous);
            return ShutdownBlockReasonDestroy(windowHandle);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

        [Flags]
        private enum ExecutionState : uint
        {
            EsAwaymodeRequired = 0x00000040,
            EsContinuous = 0x80000000,
            EsDisplayRequired = 0x00000002,
            EsSystemRequired = 0x00000001
        }

        [DllImport("user32.dll")]
        private static extern bool ShutdownBlockReasonCreate(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string pwszReason);

        [DllImport("user32.dll")]
        private static extern bool ShutdownBlockReasonDestroy(IntPtr hWnd);

        public static bool PutToSleep()
        {
            return Application.SetSuspendState(PowerState.Suspend, true, true) ||
                   Application.SetSuspendState(PowerState.Hibernate, true, true);
        }
    }
}