using System;
using System.Collections.Generic;
using System.Drawing;
using GTA;
using Font = GTA.Font;

namespace NativeUI
{
    public class UIMenuListItem : UIMenuItem, IListItem
    {
        protected UIResText _itemText;
        protected Sprite _arrowLeft;
        protected Sprite _arrowRight;

        protected int _index;
        protected List<object> _items;


        /// <summary>
        /// Triggered when the list is changed.
        /// </summary>
        public event ItemListEvent OnListChanged;

        
        /// <summary>
        /// Returns the current selected index.
        /// </summary>
        public int Index
        {
            get { return _index % Items.Count; }
            set { _index = 100000000 - (100000000 % Items.Count) + value; }
        }

        /// <summary>
        /// Returns the current selected index.
        /// </summary>
        public List<object> Items
        {
            get => _items;
            set
            {
                Index = 0;
                _items = value;
            }
        }


        /// <summary>
        /// List item, with left/right arrows.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="items">List that contains your items.</param>
        /// <param name="index">Index in the list. If unsure user 0.</param>
        public UIMenuListItem(string text, List<object> items, int index) : this(text, items, index, "")
        {
        }

        /// <summary>
        /// List item, with left/right arrows.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="items">List that contains your items.</param>
        /// <param name="index">Index in the list. If unsure user 0.</param>
        /// <param name="description">Description for this item.</param>
        public UIMenuListItem(string text, List<object> items, int index, string description) : base(text, description)
        {
            const int y = 0;
            _items = items;
            _arrowLeft = new Sprite("commonmenu", "arrowleft", new Point(110, 105 + y), new Size(30, 30));
            _arrowRight = new Sprite("commonmenu", "arrowright", new Point(280, 105 + y), new Size(30, 30));
            _itemText = new UIResText("", new Point(290, y + 104), 0.35f, Color.White, Font.ChaletLondon,
                UIResText.Alignment.Left) {TextAlignment = UIResText.Alignment.Right};
            Index = index;
        }


        /// <summary>
        /// Change item's position.
        /// </summary>
        /// <param name="y">New Y position.</param>
        public override void Position(int y)
        {
            _arrowLeft.Position = new Point(300 + Offset.X + Parent.WidthOffset, 147 + y + Offset.Y);
            _arrowRight.Position = new Point(400 + Offset.X + Parent.WidthOffset, 147 + y + Offset.Y);
            _itemText.Position = new Point(300 + Offset.X + Parent.WidthOffset, y + 147 + Offset.Y);
            base.Position(y);
        }


        /// <summary>
        /// Find an item in the list and return it's index.
        /// </summary>
        /// <param name="item">Item to search for.</param>
        /// <returns>Item index.</returns>
        [Obsolete("Use UIMenuListItem.Items.FindIndex(p => ReferenceEquals(p, item)) instead.")]
        public virtual int ItemToIndex(object item)
        {
            return Items.FindIndex(p => ReferenceEquals(p, item));
        }


        /// <summary>
        /// Find an item by it's index and return the item.
        /// </summary>
        /// <param name="index">Item's index.</param>
        /// <returns>Item</returns>
        [Obsolete("Use UIMenuListItem.Items[Index] instead.")]
        public virtual object IndexToItem(int index)
        {
            return Items[index];
        }


        /// <summary>
        /// Draw item.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            string caption = Items[Index].ToString();
            float offset = StringMeasurer.MeasureString(caption, _itemText.Font, _itemText.Scale);

            _itemText.Color = Enabled ? Selected ? Color.Black : Color.WhiteSmoke : Color.FromArgb(163, 159, 148);
            
            _itemText.Caption = caption;

            _arrowLeft.Color = Enabled ? Selected ? Color.Black : Color.WhiteSmoke : Color.FromArgb(163, 159, 148);
            _arrowRight.Color = Enabled ? Selected ? Color.Black : Color.WhiteSmoke : Color.FromArgb(163, 159, 148);

            _arrowLeft.Position = new Point(375 - (int)offset + Offset.X + Parent.WidthOffset, _arrowLeft.Position.Y);
            if (Selected)
            {
                _arrowLeft.Draw();
                _arrowRight.Draw();
                _itemText.Position = new Point(403 + Offset.X + Parent.WidthOffset, _itemText.Position.Y);
            }
            else
            {
                _itemText.Position = new Point(418 + Offset.X + Parent.WidthOffset, _itemText.Position.Y);
            }
            _itemText.Draw();
        }

        internal virtual void ListChangedTrigger(int newindex)
        {
            OnListChanged?.Invoke(this, newindex);
        }

        public override void SetRightBadge(BadgeStyle badge)
        {
            throw new Exception("UIMenuListItem cannot have a right badge.");
        }

        public override void SetRightLabel(string text)
        {
            throw new Exception("UIMenuListItem cannot have a right label.");
        }

        [Obsolete("Use UIMenuListItem.Items[Index].ToString() instead.")]
        public string CurrentItem()
        {
            return Items[Index].ToString();
        }
    }
}
