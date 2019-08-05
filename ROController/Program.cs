using ShComp;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ROController
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(RunAsync).Wait();
        }

        private static async Task RunAsync()
        {
            Console.WriteLine("取引所のウィンドウを表示してください。");

            Console.WriteLine("アイテムの個数を入力してください。");
            var count = int.Parse(Console.ReadLine());
            Console.WriteLine($"{count}個を7回に分けて出品します。");

            Console.WriteLine($"アイテムの座標をクリックしてください。");
            var point = await GetPointAsync();
            var rect = await GetWindowRectAsync(point, new CancellationTokenSource(1000).Token);
            var row = new ROWindow(rect);
            Console.WriteLine($"(X:{rect.X}, Y:{rect.Y}, W:{rect.Width}, H:{rect.Height})にゲームウィンドウが見つかりました。");
            await Task.Delay(1000);

            for (int i = 0; i < 7; i++)
            {
                var c = (count - 1) / (7 - i) + 1;
                count -= c;
                Console.WriteLine($"{i + 1}回目の出品です。{c}個出品して、残り{count}個です。");

                row.GetSellCountBox().Click();
                await Task.Delay(300);

                PressKey(0x2e);
                PressKey(25);
                await Task.Delay(300);

                SendKeys.SendWait(c.ToString());
                await Task.Delay(300);

                row.GetTextBoxOk().Click();
                await Task.Delay(300);

                row.GetSellButton().Click();
                await Task.Delay(1000);

                if (i != 6)
                {
                    point.Click();
                    await Task.Delay(300);
                }
            }
        }

        private static void PressKey(byte code)
        {
            keybd_event(code, 0, 0, 0);
            keybd_event(code, 0, KEYEVENTF_KEYUP, 0);
        }

        private static Task<Point> GetPointAsync()
        {
            var tcs = new TaskCompletionSource<Point>();
            var hooker = new MouseHooker();

            hooker.EventReceived += (status, point) =>
            {
                if (status == 514)
                {
                    tcs.TrySetResult(point);
                    Application.Exit();
                }
            };

            Task.Run(() =>
            {
                hooker.Start();
                Application.Run();
                hooker.Stop();
                Console.WriteLine("unhooked");
            });

            return tcs.Task;
        }

        private static Task<Rectangle> GetWindowRectAsync(Point point, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<Rectangle>();
            cancellationToken.Register(() => tcs.TrySetCanceled());

            EnumWindows((IntPtr hWnd, IntPtr lParam) =>
            {
                var csb = new StringBuilder(256);
                GetClassName(hWnd, csb, csb.Capacity);

                var cs = csb.ToString();
                if (cs.IndexOf("BlueStacks.exe") == -1) return true;

                RECT rect;
                GetWindowRect(hWnd, out rect);

                if (rect.Left <= point.X && rect.Right >= point.X && rect.Top <= point.Y && rect.Bottom >= point.Y)
                {
                    tcs.TrySetResult(new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top));
                    return false;
                }

                return true;
            }, IntPtr.Zero);

            return tcs.Task;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const uint KEYEVENTF_KEYUP = 0x0002;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;
    }
}
