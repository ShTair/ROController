using ShComp;
using System.Drawing;

namespace ROController
{
    class ROWindow
    {
        public Rectangle Rectangle { get; }

        public ROWindow(Rectangle rectangle)
        {
            Rectangle = rectangle;
        }

        public Point GetSellCountBox() => GetGamePoint(0.385, 0.48);

        public Point GetTextBoxOk() => GetGamePoint(0.925, 0.9);

        public Point GetSellButton() => GetGamePoint(0.415, 0.83);

        private Point GetGamePoint(double px, double py)
        {
            return new Point((int)(Rectangle.X + Rectangle.Width * px), (int)(Rectangle.Y + Rectangle.Height * py));
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
