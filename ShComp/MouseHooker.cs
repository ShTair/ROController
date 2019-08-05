using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ShComp
{
    public class MouseHooker : IDisposable
    {
        public bool Hooking { get; private set; }

        public event Action<int, Point> EventReceived;

        private IntPtr _hHook;

        public void Start()
        {
            _hHook = SetWindowsHookEx(HookType.WH_MOUSE_LL, OnHookProc, IntPtr.Zero, IntPtr.Zero);
            Hooking = true;
        }

        private IntPtr OnHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                try
                {
                    var mhs = Marshal.PtrToStructure<MouseHookStruct>(lParam);
                    EventReceived?.Invoke(wParam, new Point(mhs.pt.x, mhs.pt.y));
                }
                catch { }
            }

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

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(HookType idHook, HOOKPROC lpfn, IntPtr hMod, IntPtr dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hHook);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hHook, int nCode, int wParam, IntPtr lParam);

        private delegate IntPtr HOOKPROC(int nCode, int wParam, IntPtr lParam);

        private enum HookType : int
        {
            WH_MOUSE_LL = 14,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        #endregion
    }
}
