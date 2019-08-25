using GTA;
using GTA.Native;
using System;
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

        /// <summary>
        /// Chech whether the mouse is inside the specified rectangle.
        /// </summary>
        /// <param name="topLeft">Start point of the rectangle at the top left.</param>
        /// <param name="boxSize">size of your rectangle.</param>
        /// <returns>true if the mouse is inside of the specified bounds, false otherwise.</returns>
        public static bool IsMouseInBounds(Point topLeft, Size boxSize)
        {
            // Get the resolution while maintaining the ratio.
            SizeF res = ResolutionMantainRatio;
            // Then, get the position of mouse on the screen while relative to the current resolution
            int mouseX = (int)Math.Round(Function.Call<float>(Hash.GET_CONTROL_NORMAL, 0, (int)Control.CursorX) * res.Width);
            int mouseY = (int)Math.Round(Function.Call<float>(Hash.GET_CONTROL_NORMAL, 0, (int)Control.CursorY) * res.Height);
            // And check if the mouse is on the rectangle bounds
            bool isX = mouseX >= topLeft.X && mouseX <= topLeft.X + boxSize.Width;
            bool isY = mouseY > topLeft.Y && mouseY < topLeft.Y + boxSize.Height;
            // Finally, return the result of the checks
            return isX && isY;
        }
    }
}
