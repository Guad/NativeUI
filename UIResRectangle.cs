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
            int width = Game.ScreenResolution.Width;
            int height = Game.ScreenResolution.Height;
            /*
            const float w = static_cast<float>(this->Size.Width) / UI::WIDTH;
		const float h = static_cast<float>(this->Size.Height) / UI::HEIGHT;
		const float x = ((static_cast<float>(this->Position.X) + offset.Width) / UI::WIDTH) + w * 0.5f;
		const float y = ((static_cast<float>(this->Position.Y) + offset.Height) / UI::HEIGHT) + h * 0.5f;

		Native::Function::Call(Native::Hash::DRAW_RECT, x, y, w, h, this->Color.R, this->Color.G, this->Color.B, this->Color.A);
             */
            float w = (float) Size.Width/width;
            float h = (float) Size.Height/height;
            float x = ((Position.X + offset.Width)/(float)width) + w*0.5f;
            float y = ((Position.Y + offset.Height)/(float) height) + h*0.5f;

            Function.Call(Hash.DRAW_RECT, x, y, w, h, Color.R, Color.G, Color.B, Color.A);
        }
    }
}