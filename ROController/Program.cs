using ShComp;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ROController
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(RunAsync).Wait();

            ConsoleController.OnEventGenerated += ConsoleController_OnEventGenerated;

            Console.ReadLine();

            Mouse.Unhook();
        }

        private static bool ConsoleController_OnEventGenerated(ConsoleController.CtrlTypes ctrlType)
        {
            Console.WriteLine(ctrlType);

            Mouse.Unhook();

            Task.Delay(2000).Wait();
            return false;
        }

        private static async Task RunAsync()
        {
            EnumWindows(OnEnumWindows, IntPtr.Zero);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private static bool OnEnumWindows(IntPtr hWnd, IntPtr lParam)
        {
            //ウィンドウのクラス名を取得する
            StringBuilder csb = new StringBuilder(256);
            GetClassName(hWnd, csb, csb.Capacity);

            var cs = csb.ToString();
            if (cs.IndexOf("BlueStacks.exe") == -1) return true;

            //ウィンドウのタイトルを取得する
            int textLen = GetWindowTextLength(hWnd);
            StringBuilder tsb = new StringBuilder(textLen + 1);
            GetWindowText(hWnd, tsb, tsb.Capacity);

            //結果を表示する
            Console.WriteLine("クラス名:" + csb.ToString());
            Console.WriteLine("タイトル:" + tsb.ToString());

            RECT rect;
            GetWindowRect(hWnd, out rect);

            Console.WriteLine(rect);

            Console.WriteLine();

            //すべてのウィンドウを列挙する
            return true;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public int X
        {
            get { return Left; }
            set { Right -= (Left - value); Left = value; }
        }

        public int Y
        {
            get { return Top; }
            set { Bottom -= (Top - value); Top = value; }
        }

        public int Height
        {
            get { return Bottom - Top; }
            set { Bottom = value + Top; }
        }

        public int Width
        {
            get { return Right - Left; }
            set { Right = value + Left; }
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{X={0},Y={1},W={2},H={3}}}", Left, Top, Right - Left, Bottom - Top);
        }
    }
}
