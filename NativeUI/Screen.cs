using GTA;
using System.Drawing;

namespace NativeUI
{
    /// <summary>
    /// Tools to deal with the game screen.
    /// </summary>
    public static class Screen
    {
        /// <summary>
        /// The 1080pixels-based screen resolution while mantaining current aspect ratio.
        /// </summary>
        public static SizeF ResolutionMantainRatio
        {
            get
            {
                // Get the game width and height
                int screenw = Game.ScreenResolution.Width;
                int screenh = Game.ScreenResolution.Height;
                // Calculate the ratio
                float ratio = (float)screenw / screenh;
                // And the width with that ratio
                float width = 1080f * ratio;
                // Finally, return a SizeF
                return new SizeF(width, 1080f);
            }
        }
    }
}
