using System;
using System.Drawing;
using GTA;
using GTA.Native;
using Font = GTA.Font;

namespace NativeUI
{
    public class UIResText : UIText
    {
         public UIResText(string caption, Point position, float scale) : base(caption, position, scale)
        { }

        public UIResText(string caption, Point position, float scale, Color color) : base(caption, position, scale, color)
        { }

        public UIResText(string caption, Point position, float scale, Color color, Font font, bool centered) : base(caption, position, scale, color, font, centered)
        { }

        public bool RightJustify { get; set; }

        public static void AddLongString(string str)
        {
            const int strLen = 99;
            for (int i = 0; i < str.Length; i += strLen)
            {
                string substr = str.Substring(i, Math.Min(strLen, str.Length - i));
                Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, substr);
            }
        }

        public override void Draw(Size offset)
        {
            int screenw = Game.ScreenResolution.Width;
            int screenh = Game.ScreenResolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;

            float x = ((float) Position.X)/width;
            float y = ((float)Position.Y)/height;

            Function.Call(Hash.SET_TEXT_FONT, (int)Font);
            Function.Call(Hash.SET_TEXT_SCALE, 1.0f, Scale);
            Function.Call(Hash.SET_TEXT_COLOUR, Color.R, Color.G, Color.B, Color.A);
            if(Centered)
                Function.Call(Hash.SET_TEXT_CENTRE, true);
            else if(RightJustify)
                Function.Call(Hash.SET_TEXT_RIGHT_JUSTIFY, true);

            Function.Call(Hash._SET_TEXT_ENTRY, "jamyfafi");
            AddLongString(Caption);
            Function.Call(Hash._DRAW_TEXT, x, y);
        }
    }
}