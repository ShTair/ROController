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
        public extern static bool EnumWindows(EnumWindowsDelegate lpEnumFunc, IntPtr lparam);

        public delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lparam);

        private static bool OnEnumWindows(IntPtr hWnd, IntPtr lparam)
        {
            int textLen = GetWindowTextLength(hWnd);
            if (0 < textLen)
            {
                //ウィンドウのタイトルを取得する
                StringBuilder tsb = new StringBuilder(textLen + 1);
                GetWindowText(hWnd, tsb, tsb.Capacity);

                //ウィンドウのクラス名を取得する
                StringBuilder csb = new StringBuilder(256);
                GetClassName(hWnd, csb, csb.Capacity);

                //結果を表示する
                Console.WriteLine("クラス名:" + csb.ToString());
                Console.WriteLine("タイトル:" + tsb.ToString());
            }

            //すべてのウィンドウを列挙する
            return true;

        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd,
      StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd,
            StringBuilder lpClassName, int nMaxCount);
    }
}
