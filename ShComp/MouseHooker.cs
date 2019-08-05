using System;
using System.Runtime.InteropServices;

namespace ShComp
{
    public class MouseHooker : IDisposable
    {
        public bool Hooking { get; private set; }

        public event Action EventReceived;

        private IntPtr _hHook;

        public void Start()
        {
            var module = LoadLibrary("user32.dll");
            _hHook = SetWindowsHookEx(HookType.WH_MOUSE_LL, OnHookProc, IntPtr.Zero, IntPtr.Zero);
            Hooking = true;
        }

        private IntPtr OnHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                Console.WriteLine($"{nCode} {wParam} {lParam}");
                EventReceived?.Invoke();
            }
            catch { }

            return CallNextHookEx(_hHook, nCode, wParam, lParam);
        }

        public void Stop()
        {
            if (Hooking)
            {
                Hooking = false;
                UnhookWindowsHookEx(_hHook);
            }
        }

        public void Dispose()
        {
            Stop();
        }

        #region Api

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(HookType idHook, HOOKPROC lpfn, IntPtr hMod, IntPtr dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hHook);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hHook, int nCode, IntPtr wParam, IntPtr lParam);

        private delegate IntPtr HOOKPROC(int nCode, IntPtr wParam, IntPtr lParam);

        private enum HookType : int
        {
            WH_MOUSE_LL = 14,
        }

        #endregion
    }
}
