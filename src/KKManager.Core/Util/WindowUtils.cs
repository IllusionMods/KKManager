using System;
using System.Runtime.InteropServices;

namespace KKManager.Util
{
    public class WindowUtils
    {
        [Flags]
        public enum FlashWindowFlags : uint
        {
            /// <summary>
            /// Stop flashing. The system restores the window to its original stae.
            /// </summary>
            FLASHW_STOP = 0,

            /// <summary>
            /// Flash the window caption.
            /// </summary>
            FLASHW_CAPTION = 1,

            /// <summary>
            /// Flash the taskbar button.
            /// </summary>
            FLASHW_TRAY = 2,

            /// <summary>
            /// Flash both the window caption and taskbar button.
            /// This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
            /// </summary>
            FLASHW_ALL = 3,

            /// <summary>
            /// Flash continuously, until the FLASHW_STOP flag is set.
            /// </summary>
            FLASHW_TIMER = 4,

            /// <summary>
            /// Flash continuously until the window comes to the foreground.
            /// </summary>
            FLASHW_TIMERNOFG = 12
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        /// <param name="hWnd"> Window to flash. </param>
        /// <param name="flags"> The Flash Status. </param>
        /// <param name="timeout">
        /// The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the
        /// default cursor blink rate.
        /// </param>
        /// <param name="flashCount"> The number of times to Flash the window. Each flash is separated by the timeout delay. </param>
        public static void FlashWindow(IntPtr hWnd, FlashWindowFlags flags = FlashWindowFlags.FLASHW_ALL | FlashWindowFlags.FLASHW_TIMERNOFG, uint flashCount = 60, uint timeout = 0)
        {
            var fInfo = new FLASHWINFO();

            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hWnd;
            fInfo.dwFlags = (uint)flags;
            fInfo.uCount = flashCount;
            fInfo.dwTimeout = timeout;

            FlashWindowEx(ref fInfo);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FLASHWINFO
        {
            /// <summary>
            /// The size of the structure in bytes.
            /// </summary>
            public uint cbSize;

            /// <summary>
            /// A Handle to the Window to be Flashed. The window can be either opened or minimized.
            /// </summary>
            public IntPtr hwnd;

            /// <summary>
            /// The Flash Status.
            /// </summary>
            public uint dwFlags;

            /// <summary>
            /// The number of times to Flash the window.
            /// </summary>
            public uint uCount;

            /// <summary>
            /// The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the default cursor blink
            /// rate.
            /// </summary>
            public uint dwTimeout;
        }
    }
}
