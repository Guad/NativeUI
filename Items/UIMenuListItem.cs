using System;
using System.Collections.Generic;
using System.Drawing;
using CitizenFX.Core.UI;

namespace NativeUI
{
    public class UIMenuListItem : UIMenuItem, IListItem
    {
        protected UIResText _itemText;
        protected Sprite _arrowLeft;
        protected Sprite _arrowRight;

        protected int _index;
        protected List<dynamic> _items;


        /// <summary>
        /// Triggered when the list is changed.
        /// </summary>
        public event ItemListEvent OnListChanged;

        /// <summary>		
        /// Triggered when a list item is selected.		
        /// </summary>		
        public event ItemListEvent OnListSelected;

        /// <summary>
        /// Returns the current selected index.
        /// </summary>
        public int Index
        {
            get { return _index % _items.Count; }
            set { _index = 100000000 - (100000000 % _items.Count) + value; }
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
            const int y = 0;
            _items = items;
            _arrowLeft = new Sprite("commonmenu", "arrowleft", new PointF(110, 105 + y), new SizeF(30, 30));
            _arrowRight = new Sprite("commonmenu", "arrowright", new PointF(280, 105 + y), new SizeF(30, 30));
            _itemText = new UIResText("", new PointF(290, y + 104), 0.35f, UnknownColors.White, Font.ChaletLondon,
                UIResText.Alignment.Left)
            { TextAlignment = UIResText.Alignment.Right };
            Index = index;
        }


        /// <summary>
        /// Change item's position.
        /// </summary>
        /// <param name="y">New Y position.</param>
        public override void Position(int y)
        {
            _arrowLeft.Position = new PointF(300 + Offset.X + Parent.WidthOffset, 147 + y + Offset.Y);
            _arrowRight.Position = new PointF(400 + Offset.X + Parent.WidthOffset, 147 + y + Offset.Y);
            _itemText.Position = new PointF(300 + Offset.X + Parent.WidthOffset, y + 147 + Offset.Y);
            base.Position(y);
        }


        /// <summary>
        /// Find an item in the list and return it's index.
        /// </summary>
        /// <param name="item">Item to search for.</param>
        /// <returns>Item index.</returns>
        public virtual int ItemToIndex(dynamic item)
        {
            return _items.FindIndex(p => ReferenceEquals(p, item));
        }


        /// <summary>
        /// Find an item by it's index and return the item.
        /// </summary>
        /// <param name="index">Item's index.</param>
        /// <returns>Item</returns>
        public virtual dynamic IndexToItem(int index)
        {
            return _items[index];
        }


        /// <summary>
        /// Draw item.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            string caption = _items[Index].ToString();
            float offset = StringMeasurer.MeasureString(caption);

            _itemText.Color = Enabled ? Selected ? UnknownColors.Black : UnknownColors.WhiteSmoke : Color.FromArgb(163, 159, 148);

            _itemText.Caption = caption;

            _arrowLeft.Color = Enabled ? Selected ? UnknownColors.Black : UnknownColors.WhiteSmoke : Color.FromArgb(163, 159, 148);
            _arrowRight.Color = Enabled ? Selected ? UnknownColors.Black : UnknownColors.WhiteSmoke : Color.FromArgb(163, 159, 148);

            _arrowLeft.Position = new PointF(375 - (int)offset + Offset.X + Parent.WidthOffset, _arrowLeft.Position.Y);
            if (Selected)
            {
                _arrowLeft.Draw();
                _arrowRight.Draw();
                _itemText.Position = new PointF(403 + Offset.X + Parent.WidthOffset, _itemText.Position.Y);
            }
            else
            {
                _itemText.Position = new PointF(418 + Offset.X + Parent.WidthOffset, _itemText.Position.Y);
            }
            _itemText.Draw();
        }

        internal virtual void ListChangedTrigger(int newindex)
        {
            OnListChanged?.Invoke(this, newindex);
        }

        internal virtual void ListSelectedTrigger(int newindex)
        {
            OnListSelected?.Invoke(this, newindex);
        }

        public override void SetRightBadge(BadgeStyle badge)
        {
            throw new Exception("UIMenuListItem cannot have a right badge.");
        }

        public override void SetRightLabel(string text)
        {
            throw new Exception("UIMenuListItem cannot have a right label.");
        }

        public string CurrentItem()
        {
            return _items[Index].ToString();
        }
    }
}