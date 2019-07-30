using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ShComp
{
    public static partial class Mouse
    {
        private static IntPtr _hHook;

        public static void SetHook()
        {
            var module = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);

            _hHook = SetWindowsHookEx(HookType.WH_MOUSE_LL, OnHookProc, module, IntPtr.Zero);
        }

        public static void Unhook()
        {
            UnhookWindowsHookEx(_hHook);
        }

        private static IntPtr OnHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            { }
            catch { }

            return CallNextHookEx(_hHook, nCode, wParam, lParam);
        }

        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(HookType idHook, HOOKPROC lpfn, IntPtr hMod, IntPtr dwThreadId);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hHook);

        public const int HC_ACTION = 0;
        public delegate IntPtr HOOKPROC(int nCode, IntPtr wParam, IntPtr lParam);

        public enum HookType : int
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
        public static extern IntPtr CallNextHookEx(IntPtr hHook, int nCode, IntPtr wParam, IntPtr lParam);
    }
}
