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
            // Get the Width and Heigth of the current resolution
            float W = Game.ScreenResolution.Width;
            float H = Game.ScreenResolution.Height;
            // Calculate the ratio of the current resolution
            // In other words, the height percentage relative to the width
            float Ratio = W / H;
            // Get the real width
            Width = 1080f * Ratio;
            // Then, return the correctly calculated PointF
            return new PointF(point.X / Width, point.Y / 1080f);
        }
    }
}
