using System;
using System.Collections.Generic;
using System.Drawing;
using GTA;
using GTA.Native;

namespace NativeUI
{
    public class UIMenuListItem : UIMenuItem
    {
        private UIResText itemText;

        private Sprite _arrowLeft;
        private Sprite _arrowRight;

        private List<dynamic> Items;

        public event ItemListEvent OnListChanged;

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
            _arrowLeft = new Sprite("commonmenu", "arrowleft", new Point(110, 105 + y), new Size(30, 30));
            _arrowRight = new Sprite("commonmenu", "arrowright", new Point(280, 105 + y), new Size(30, 30));
            itemText = new UIResText("", new Point(290, y + 104), 0.33f, Color.White, GTA.Font.ChaletLondon, false);
            
            Index = index;
        }


        /// <summary>
        /// Change item's position.
        /// </summary>
        /// <param name="y">New Y position.</param>
        public override void Position(int y)
        {
            _arrowLeft.Position = new Point(300 + Offset.X, 148 + y + Offset.Y);
            _arrowRight.Position = new Point(400 + Offset.X, 148 + y + Offset.Y);
            itemText.Position = new Point(300 + Offset.X, y + 149 + Offset.Y);
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
            Function.Call((Hash)0x54CE8AC98E120CAB, "jamyfafi");
            UIResText.AddLongString(caption);
            int screenw = Game.ScreenResolution.Width;
            int screenh = Game.ScreenResolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;
            int offset = Convert.ToInt32(Function.Call<float>((Hash)0x85F061DA64ED2F67, (int)0) * width * 0.33f);

            itemText.Color = Selected ? Color.Black : Color.WhiteSmoke;
            
            itemText.Caption = caption;

            _arrowLeft.Color = Selected ? Color.Black : Color.WhiteSmoke;
            _arrowRight.Color = Selected ? Color.Black : Color.WhiteSmoke;

            _arrowLeft.Position = new Point(380 - offset + Offset.X, _arrowLeft.Position.Y);
            if (Selected)
            {
                _arrowLeft.Draw();
                _arrowRight.Draw();
                itemText.Position = new Point(410 - offset + Offset.X, itemText.Position.Y);
            }
            else
            {
                itemText.Position = new Point(430 - offset + Offset.X, itemText.Position.Y);
            }
            itemText.Draw();
        }

        internal virtual void ListChangedTrigger(int newindex)
        {
            OnListChanged?.Invoke(this, newindex);
        }
    }
}