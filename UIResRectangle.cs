using CitizenFX.Core.UI;
using System.Drawing;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace NativeUI
{
    /// <summary>
    /// A rectangle in 1080 pixels height system.
    /// </summary>
    public class UIResRectangle : Rectangle
    {
        public UIResRectangle()
        { }

        public UIResRectangle(Point pos, Size size) : base(pos, size)
        { }

        public UIResRectangle(Point pos, Size size, Color color) : base(pos, size, color)
        { }
        
        public void Draw(Size offset)
        {
            if (!Enabled) return;
            int screenw = 1920;// Game.ScreenResolution.Width;
            int screenh = 1080;// Game.ScreenResolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;

            float w = Size.Width/width;
            float h = Size.Height/height;
            float x = ((Position.X + offset.Width)/width) + w*0.5f;
            float y = ((Position.Y + offset.Height)/height) + h*0.5f;

            Function.Call(Hash.DRAW_RECT, x, y, w, h, Color.R, Color.G, Color.B, Color.A);
        }
    }
}