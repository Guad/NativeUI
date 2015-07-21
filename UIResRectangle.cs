using System.Drawing;
using GTA;
using GTA.Native;

namespace NativeUI
{
    public class UIResRectangle : UIRectangle
    {
        public UIResRectangle() : base()
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

            float w = (float) Size.Width/width;
            float h = (float) Size.Height/height;
            float x = ((Position.X + offset.Width)/(float)width) + w*0.5f;
            float y = ((Position.Y + offset.Height)/(float) height) + h*0.5f;

            Function.Call(Hash.DRAW_RECT, x, y, w, h, Color.R, Color.G, Color.B, Color.A);
        }
    }
}