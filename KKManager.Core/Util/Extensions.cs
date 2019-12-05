using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace KKManager.Util
{
    public static class Extensions
    {
        public static void SafeInvoke(this Control obj, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (!obj.IsDisposed && !obj.Disposing)
            {
                if (obj.InvokeRequired)
                {
                    try
                    {
                        obj.Invoke(action);
                    }
                    catch (ObjectDisposedException) { }
                    catch (InvalidOperationException) { }
                    catch (InvalidAsynchronousStateException) { }
                }
                else
                {
                    action();
                }
            }
        }
    }
}
