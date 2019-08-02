using ShComp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROController
{
    class ROWindow
    {
        public int X { get; }

        public int Y { get; }

        public int Width { get; }

        public int Height { get; }

        public ROWindow(int left, int top, int right, int bottom)
        {
            X = left;
            Y = top;
            Width = right - left;
            Height = bottom - top;
        }
    }

    static class PointExtension
    {
        public static void Click(this Point point)
        {
            Mouse.Click(point.X, point.Y);
        }

        public static void Move(this Point point)
        {
            Mouse.Move(point.X, point.Y);
        }
    }
}
