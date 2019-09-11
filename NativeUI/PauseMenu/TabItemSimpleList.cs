﻿using CitizenFX.Core.UI;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace NativeUI.PauseMenu
{
    public class TabItemSimpleList : TabItem
    {
        public TabItemSimpleList(string title, Dictionary<string, string> dict) : base(title)
        {
            Dictionary = dict;
            DrawBg = false;
        }

        public Dictionary<string, string> Dictionary { get; set; }

        public override void Draw()
        {
            base.Draw();

            var res = Screen.ResolutionMaintainRatio;

            var alpha = (Focused || !CanBeFocused) ? 180 : 60;
            var blackAlpha = (Focused || !CanBeFocused) ? 200 : 90;
            var fullAlpha = (Focused || !CanBeFocused) ? 255 : 150;

            var rectSize = (int)(BottomRight.X - TopLeft.X);

            for (int i = 0; i < Dictionary.Count; i++)
            {
                new UIResRectangle(new Point(TopLeft.X, TopLeft.Y + (40 * i)),
                    new Size(rectSize, 40), i % 2 == 0 ? Color.FromArgb(alpha, 0, 0, 0) : Color.FromArgb(blackAlpha, 0, 0, 0)).Draw();

                var item = Dictionary.ElementAt(i);

                new UIResText(item.Key, new Point(TopLeft.X + 6, TopLeft.Y + 5 + (40 * i)), 0.35f, Color.FromArgb(fullAlpha, Colors.White)).Draw();
                new UIResText(item.Value, new Point(BottomRight.X - 6, TopLeft.Y + 5 + (40 * i)), 0.35f, Color.FromArgb(fullAlpha, Colors.White), CitizenFX.Core.UI.Font.ChaletLondon, Alignment.Right).Draw();
            }
        }
    }
}