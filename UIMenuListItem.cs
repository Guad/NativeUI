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
            get { return _index % Items.Count; }
            set { _index = 100000 - (100000 % Items.Count) + value; }
        }


        /// <summary>
        /// List item, with left/right arrows.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="items">List that contains your items.</param>
        /// <param name="index">Index in the list. If unsure user 0.</param>
        public UIMenuListItem(string text, List<dynamic> items, int index)
            : this(text, items, index, "")
        {
        }

        /// <summary>
        /// List item, with left/right arrows.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="items">List that contains your items.</param>
        /// <param name="index">Index in the list. If unsure user 0.</param>
        /// <param name="description">Description for this item.</param>
        public UIMenuListItem(string text, List<dynamic> items, int index, string description)
            : base(text, description)
        {
            int y = 0;
            Items = new List<dynamic>(items);
            _arrowLeft = new Sprite("commonmenu", "arrowleft", new Point(110, 108 + y), new Size(10, 10));
            _arrowRight = new Sprite("commonmenu", "arrowright", new Point(265, 108 + y), new Size(10, 10));
            itemText = new UIText("", new Point(290, y + 104), 0.3f, Color.White, GTA.Font.ChaletLondon, false);
            Index = index;
        }


        /// <summary>
        /// Change item's position.
        /// </summary>
        /// <param name="y">New Y position.</param>
        public override void Position(int y)
        {
            _arrowLeft.Position = new Point(110 + Offset.X, 108 + y + Offset.Y);
            _arrowRight.Position = new Point(265 + Offset.X, 108 + y + Offset.Y);
            itemText.Position = new Point(290 + Offset.X, y + 104 + Offset.Y);
            base.Position(y);
        }


        /// <summary>
        /// Find an item in the list and return it's index.
        /// </summary>
        /// <param name="item">Item to search for.</param>
        /// <returns>Item index.</returns>
        public int ItemToIndex(dynamic item)
        {
            return Items.FindIndex(item);
        }


        /// <summary>
        /// Find an item by it's index and return the item.
        /// </summary>
        /// <param name="index">Item's index.</param>
        /// <returns>Item</returns>
        public dynamic IndexToItem(int index)
        {
            return Items[index];
        }


        /// <summary>
        /// Draw item.
        /// </summary>
        public override void Draw()
        {
            base.Draw();
            string caption = Items[Index % Items.Count].ToString();

            SizeF strSize;
            using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
            {
                strSize = g.MeasureString(caption, new System.Drawing.Font("Helvetica", 11, FontStyle.Regular, GraphicsUnit.Pixel));
            }
            int offset = Convert.ToInt32(strSize.Width);

            itemText.Color = Selected ? Color.Black : Color.WhiteSmoke;
            itemText.Position = new Point(262 - offset + Offset.X, itemText.Position.Y);
            itemText.Caption = caption;

            _arrowLeft.Color = Selected ? Color.Black : Color.WhiteSmoke;
            _arrowRight.Color = Selected ? Color.Black : Color.WhiteSmoke;

            _arrowLeft.Position = new Point(250 - offset + Offset.X, _arrowLeft.Position.Y);

            _arrowLeft.Draw();
            _arrowRight.Draw();
            itemText.Draw();
        }
    }
}