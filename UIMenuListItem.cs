using System;
using System.Collections.Generic;
using System.Drawing;
using GTA;

namespace NativeUI
{
    public class UIMenuListItem : UIMenuItem
    {
        private UIText itemText;

        private Sprite _arrowLeft;
        private Sprite _arrowRight;

        private List<dynamic> Items;

        private int _index = 0;

        public int Index
        {
            get
            { return _index % Items.Count; }
            set { _index = 100000 - (100000 % Items.Count) + value; }
        }

        public UIMenuListItem(string text, List<dynamic> items, int index)
            : base(text)
        {
            int y = 0;
            Items = new List<dynamic>(items);
            _arrowLeft = new Sprite("commonmenu", "arrowleft", new Point(120, 93 + y), new Size(20, 20));
            _arrowRight = new Sprite("commonmenu", "arrowright", new Point(275, 93 + y), new Size(20, 20));

            itemText = new UIText("", new Point(300, y + 94), 0.3f, Color.White, GTA.Font.ChaletLondon, false);
            Index = index;
        }

        public override void Position(int y)
        {
            _arrowLeft.Position = new Point(120 + Offset.X, 93 + y + Offset.Y);
            _arrowRight.Position = new Point(275 + Offset.X, 93 + y + Offset.Y);
            itemText.Position = new Point(300 + Offset.X, y + 94 + Offset.Y);
            base.Position(y);
        }

        public int ItemToIndex(dynamic item)
        {
            return Items.FindIndex(item);
        }

        public string IndexToItem(int index)
        {
            return Items[index].ToString();
        }

        public override void Draw()
        {
            base.Draw();
            string caption = Items[Index % Items.Count].ToString();

            SizeF strSize;
            using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
            {
                strSize = g.MeasureString(caption, new System.Drawing.Font("Segoe UI", 11, FontStyle.Regular, GraphicsUnit.Pixel));
            }
            int offset = Convert.ToInt32(strSize.Width);
            //int offset = caption.Length * 5;

            itemText.Color = Selected ? Color.Black : Color.WhiteSmoke;
            itemText.Position = new Point(275 - 10 - offset + Offset.X, itemText.Position.Y + Offset.Y);
            itemText.Caption = caption;

            _arrowLeft.Color = Selected ? Color.Black : Color.WhiteSmoke;
            _arrowRight.Color = Selected ? Color.Black : Color.WhiteSmoke;

            _arrowLeft.Position = new Point(270 - 30 - offset + Offset.X, _arrowLeft.Position.Y + Offset.Y);

            _arrowLeft.Draw();
            _arrowRight.Draw();
            itemText.Draw();
        }
    }
}