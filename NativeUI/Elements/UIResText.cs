﻿using System;
using System.Drawing;
using GTA.Native;
using Font = GTA.UI.Font;
using Alignment = GTA.UI.Alignment;

namespace NativeUI
{
    /// <summary>
    /// A Text object in the 1080 pixels height base system.
    /// </summary>
    public class UIResText : GTA.UI.TextElement
    {
        public UIResText(string caption, PointF position, float scale) : base(caption, position, scale)
        {
            TextAlignment = Alignment.Left;
        }

        public UIResText(string caption, PointF position, float scale, Color color) : base(caption, position, scale, color)
        {
            TextAlignment = Alignment.Left;
        }

        public UIResText(string caption, PointF position, float scale, Color color, Font font, Alignment justify) : base(caption, position, scale, color, font, justify)
        {
            TextAlignment = justify;
        }


        public Alignment TextAlignment { get; set; }

        /// <summary>
        /// Push a long string into the stack.
        /// </summary>
        /// <param name="str"></param>
        public static void AddLongString(string str)
        {
            var utf8ByteCount = System.Text.Encoding.UTF8.GetByteCount(str);

            if (utf8ByteCount == str.Length)
            {
                AddLongStringForAscii(str);
            }
            else
            {
                AddLongStringForUtf8(str);
            }
        }

        private static void AddLongStringForAscii(string input)
        {
            const int maxByteLengthPerString = 99;

            for (int i = 0; i < input.Length; i += maxByteLengthPerString)
            {
                string substr = (input.Substring(i, Math.Min(maxByteLengthPerString, input.Length - i)));
                Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, substr);
            }
        }

        internal static void AddLongStringForUtf8(string input)
        {
            const int maxByteLengthPerString = 99;

            if (maxByteLengthPerString < 0)
            {
                throw new ArgumentOutOfRangeException("maxLengthPerString");
            }
            if (string.IsNullOrEmpty(input) || maxByteLengthPerString == 0)
            {
                return;
            }

            var enc = System.Text.Encoding.UTF8;

            var utf8ByteCount = enc.GetByteCount(input);
            if (utf8ByteCount < maxByteLengthPerString)
            {
                Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, input);
                return;
            }

            var startIndex = 0;

            for (int i = 0; i < input.Length; i++)
            {
                var length = i - startIndex;
                if (enc.GetByteCount(input.Substring(startIndex, length)) > maxByteLengthPerString)
                {
                    string substr = (input.Substring(startIndex, length - 1));
                    Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, substr);

                    i -= 1;
                    startIndex = (startIndex + length - 1);
                }
            }
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, input.Substring(startIndex, input.Length - startIndex));
        }

        [Obsolete("Use Screen.GetTextWidth instead.", true)]
        public static float MeasureStringWidth(string str, Font font, float scale) => Screen.GetTextWidth(str, font, scale);

        [Obsolete("Use Screen.GetTextWidth instead.", true)]
        public static float MeasureStringWidthNoConvert(string str, Font font, float scale) => Screen.GetTextWidth(str, font, scale);

        /// <summary>
        /// Width of the text wrap box. Set to zero to disable.
        /// </summary>
        public int Wrap { get; set; } = 0;
        /// <summary>
        /// Size of the text wrap box.
        /// </summary>
        [Obsolete("Use UIResText.Wrap instead.", true)]
        public Size WordWrap
        {
            get => new Size(Wrap, 0);
            set => Wrap = value.Width;
        }

        public Size WordWrap { get; set; }

        public override void Draw(SizeF offset)

        {
			var res = GTA.UI.Screen.Resolution;
			int screenw = res.Width;
            int screenh = res.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;

            float x = (Position.X) / width;
            float y = (Position.Y) / height;

            Function.Call(Hash.SET_TEXT_FONT, (int)Font);
            Function.Call(Hash.SET_TEXT_SCALE, 1.0f, Scale);
            Function.Call(Hash.SET_TEXT_COLOUR, Color.R, Color.G, Color.B, Color.A);
            if (Shadow)
                Function.Call(Hash.SET_TEXT_DROP_SHADOW);
            if (Outline)
                Function.Call(Hash.SET_TEXT_OUTLINE);
            switch (TextAlignment)
            {
                case Alignment.Center:
                    Function.Call(Hash.SET_TEXT_CENTRE, true);
                    break;
                case Alignment.Right:
                    Function.Call(Hash.SET_TEXT_RIGHT_JUSTIFY, true);
                    Function.Call(Hash.SET_TEXT_WRAP, 0, x);
                    break;
            }

            if (Wrap != 0)
            {
                float xsize = (Position.X + Wrap) / width;
                Function.Call(Hash.SET_TEXT_WRAP, x, xsize);
            }

            Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_TEXT, "jamyfafi");
            AddLongString(Caption);

            Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_TEXT, x, y);
        }

        public static void Draw(string caption, int xPos, int yPos, Font font, float scale, Color color, Alignment alignment, bool dropShadow, bool outline, int wordWrap)
        {
			var res = GTA.UI.Screen.Resolution;
			int screenw = res.Width;
            int screenh = res.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;

            float x = (xPos) / width;
            float y = (yPos) / height;

            Function.Call(Hash.SET_TEXT_FONT, (int)font);
            Function.Call(Hash.SET_TEXT_SCALE, 1.0f, scale);
            Function.Call(Hash.SET_TEXT_COLOUR, color.R, color.G, color.B, color.A);
            if (dropShadow)
                Function.Call(Hash.SET_TEXT_DROP_SHADOW);
            if (outline)
                Function.Call(Hash.SET_TEXT_OUTLINE);
            switch (alignment)
            {
                case Alignment.Center:
                    Function.Call(Hash.SET_TEXT_CENTRE, true);
                    break;
                case Alignment.Right:
                    Function.Call(Hash.SET_TEXT_RIGHT_JUSTIFY, true);
                    Function.Call(Hash.SET_TEXT_WRAP, 0, x);
                    break;
            }

            if (wordWrap != 0)
            {
                float xsize = (xPos + wordWrap) / width;
                Function.Call(Hash.SET_TEXT_WRAP, x, xsize);
            }

            Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_TEXT, "jamyfafi");
            AddLongString(caption);

            Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_TEXT, x, y);
        }
    }
}
