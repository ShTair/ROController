using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShComp
{
    public static partial class Mouse
    {
        public static bool Hooked { get; set; }
        private static IntPtr _hHook;

        public static event Action EventReceived;

        public static void Hook()
        {
            var module = LoadLibrary("user32.dll");

            _hHook = SetWindowsHookEx(HookType.WH_MOUSE_LL, OnHookProc, IntPtr.Zero, IntPtr.Zero);
            Hooked = true;
            Console.WriteLine(_hHook);
            var hr = Marshal.GetLastWin32Error();
            Console.WriteLine(hr);
            Application.Run();
        }

        public static void Unhook()
        {
            if (Hooked)
            {
                UnhookWindowsHookEx(_hHook);
                Hooked = false;
                Application.Exit();
            }
        }

        private static IntPtr OnHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                Console.WriteLine("recv");
                EventReceived?.Invoke();
            }
            catch { }

            return CallNextHookEx(_hHook, nCode, wParam, lParam);
        }

        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(HookType idHook, HOOKPROC lpfn, IntPtr hMod, IntPtr dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hHook);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThreadId();

        private const int HC_ACTION = 0;
        private delegate IntPtr HOOKPROC(int nCode, IntPtr wParam, IntPtr lParam);

        private enum HookType : int
        {
            WH_MSGFILTER = -1,
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14,
        }

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr LoadLibrary(string lpFileName);
    }
}
