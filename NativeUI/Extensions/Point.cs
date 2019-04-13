using GTA;
using System.Drawing;

namespace NativeUI.Extensions
{
    public static class PointExtensions
    {
        /// <summary>
        /// Converts an absolute Point to a GTA relative one.
        /// </summary>
        public static PointF ToRelative(this Point point, out float Width)
        {
            float W = Game.ScreenResolution.Width;
            float H = Game.ScreenResolution.Height;
            float Ratio = W / H;
            Width = 1080f * Ratio;
            return new PointF(point.X / Width, point.Y / 1080f);
        }
    }
}
