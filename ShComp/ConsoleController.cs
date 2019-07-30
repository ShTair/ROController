using System;
using System.Runtime.InteropServices;

namespace ShComp
{
    public static class ConsoleController
    {
        public static event ConsoleCtrlDelegate OnEventGenerated
        {
            add { SetConsoleCtrlHandler(value, true); }
            remove { throw new NotSupportedException(); }
        }

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool Add);

        public delegate bool ConsoleCtrlDelegate(CtrlTypes ctrlType);

        public enum CtrlTypes : uint
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }
    }
}
