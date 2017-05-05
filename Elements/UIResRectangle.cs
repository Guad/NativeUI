using System.Drawing;
using GTA;
using GTA.Native;

namespace NativeUI
{
    /// <summary>
    /// A rectangle in 1080 pixels height system.
    /// </summary>
    public class UIResRectangle : UIRectangle
    {
        public UIResRectangle()
        { }

        public UIResRectangle(Point pos, Size size) : base(pos, size)
        { }

        public UIResRectangle(Point pos, Size size, Color color) : base(pos, size, color)
        { }
        
        public override void Draw(Size offset)
        {
            if (!Enabled) return;
            int screenw = Game.ScreenResolution.Width;
            int screenh = Game.ScreenResolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;

            float w = Size.Width/width;
            float h = Size.Height/height;
            float x = ((Position.X + offset.Width)/width) + w*0.5f;
            float y = ((Position.Y + offset.Height)/height) + h*0.5f;

            Function.Call(Hash.DRAW_RECT, x, y, w, h, Color.R, Color.G, Color.B, Color.A);
        }

        public static void Draw(int xPos, int yPos, int boxWidth, int boxHeight, Color color)
        {
            int screenw = Game.ScreenResolution.Width;
            int screenh = Game.ScreenResolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;

            float w = boxWidth / width;
            float h = boxHeight / height;
            float x = ((xPos) / width) + w * 0.5f;
            float y = ((yPos) / height) + h * 0.5f;

            Function.Call(Hash.DRAW_RECT, x, y, w, h, color.R, color.G, color.B, color.A);
        }
    }
}