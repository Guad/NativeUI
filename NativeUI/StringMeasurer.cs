using System;
using GTA;

namespace NativeUI
{
    public static class StringMeasurer
    {
        /// <summary>
        /// Measures width of a 0.35 scale string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Obsolete("Use Screen.GetTextWidth instead.", true)]
        public static int MeasureString(string input) => (int)Screen.GetTextWidth(input, Font.ChaletLondon, 1f);

        [Obsolete("Use Screen.GetTextWidth instead.", true)]
        public static float MeasureString(string input, Font font, float scale) => Screen.GetTextWidth(input, font, scale);
    }
}
