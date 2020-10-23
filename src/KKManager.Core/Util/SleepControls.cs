using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KKManager.Util
{
    public static class SleepControls
    {
        public static void PreventSleep()
        {
            SetThreadExecutionState(ExecutionState.EsContinuous | ExecutionState.EsSystemRequired);
        }

        public static void AllowSleep()
        {
            SetThreadExecutionState(ExecutionState.EsContinuous);
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

        public static bool PutToSleep()
        {
            return Application.SetSuspendState(PowerState.Suspend, true, true) ||
                   Application.SetSuspendState(PowerState.Hibernate, true, true);
        }
    }
}