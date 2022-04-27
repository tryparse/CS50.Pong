using Microsoft.Xna.Framework;

namespace CS50.Pong
{
    class ScreenParameters
    {
        public int RealWidth { get; }
        public int RealHeight { get; }
        public int VirtualWidth { get; }
        public int VirtualHeight { get; }

        public bool IsVirtual => RealWidth != VirtualWidth || RealHeight != VirtualHeight;

        public Rectangle ScreenRectangle { get; }

        public ScreenParameters(int realWidth, int realHeight, int virtualWidth, int virtualHeight)
        {
            RealWidth = realWidth;
            RealHeight = realHeight;
            VirtualWidth = virtualWidth;
            VirtualHeight = virtualHeight;

            ScreenRectangle = new Rectangle(Point.Zero, new Point(realWidth, realHeight));
        }

        public ScreenParameters(int realWidth, int realHeight) : this(realWidth, realHeight, realWidth, realHeight)
        {

        }
    }
}