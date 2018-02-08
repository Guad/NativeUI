using System;
using System.Collections.Generic;


using Font = CitizenFX.Core.UI.Font;
using System.Drawing;

namespace NativeUI
{
    public class UIMenuSliderItem : UIMenuItem
    {

        protected Sprite _arrowLeft;
        protected Sprite _arrowRight;

        protected UIResRectangle _rectangleBackground;
        protected UIResRectangle _rectangleSlider;
        protected UIResRectangle _rectangleDivider;

        protected List<dynamic> _items;


        /// <summary>
        /// Triggered when the slider is changed.
        /// </summary>
        public event ItemSliderEvent OnSliderChanged;

        /// <summary>
        /// Triggered when a list item is selected.
        /// </summary>
        public event ItemSliderEvent OnSliderSelected;

        protected int _index;
        
        /// <summary>
        /// Returns the current selected index.
        /// </summary>
        public int Index
        {
            get { return _index % _items.Count; }
            set { _index = 100000000 - (100000000 % _items.Count) + value; }
        }


        /// <summary>
        /// List item, with slider.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="items">List that contains your items.</param>
        /// <param name="index">Index in the list. If unsure user 0.</param>
        public UIMenuSliderItem(string text, List<dynamic> items, int index)
            : this(text, items, index, "", false)
        {
        }

        /// <summary>
        /// List item, with slider.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="items">List that contains your items.</param>
        /// <param name="index">Index in the list. If unsure user 0.</param>
        /// <param name="description">Description for this item.</param>
        public UIMenuSliderItem(string text, List<dynamic> items, int index, string description)
            : this(text, items, index, description, false)
        {
        }

        /// <summary>
        /// List item, with slider.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="items">List that contains your items.</param>
        /// <param name="index">Index in the list. If unsure user 0.</param>
        /// <param name="description">Description for this item.</param>
        /// /// <param name="divider">Put a divider in the center of the slider</param>
        public UIMenuSliderItem(string text, List<dynamic> items, int index, string description, bool divider)
            : base(text, description)
        {
            const int y = 0;
            _items = new List<dynamic>(items);
            _arrowLeft = new Sprite("commonmenutu", "arrowleft", new PointF(0, 105 + y), new SizeF(15, 15));
            _arrowRight = new Sprite("commonmenutu", "arrowright", new PointF(0, 105 + y), new SizeF(15, 15));
            _rectangleBackground = new UIResRectangle(new PointF(0, 0), new SizeF(150, 9), Color.FromArgb(255, 4, 32, 57));
            _rectangleSlider = new UIResRectangle(new PointF(0, 0), new SizeF(75, 9), Color.FromArgb(255, 57, 116, 200));
            if (divider)
            {
                _rectangleDivider = new UIResRectangle(new PointF(0, 0), new SizeF(2.5f, 20), UnknownColors.WhiteSmoke);
            }
            else
            {
                _rectangleDivider = new UIResRectangle(new PointF(0, 0), new SizeF(2.5f, 20), UnknownColors.Transparent);
            }
            Index = index;
        }

        /// <summary>
        /// Change item's position.
        /// </summary>
        /// <param name="y">New Y position.</param>
        public override void Position(int y)
        {
            _rectangleBackground.Position = new PointF(250 + Offset.X, y + 158.5f + Offset.Y);
            _rectangleSlider.Position = new PointF(250 + Offset.X, y + 158.5f + Offset.Y);
            _rectangleDivider.Position = new PointF(323.5f + Offset.X, y + 153 + Offset.Y);
            _arrowLeft.Position = new PointF(235 + Offset.X + Parent.WidthOffset, 155.5f + y + Offset.Y);
            _arrowRight.Position = new PointF(400 + Offset.X + Parent.WidthOffset, 155.5f + y + Offset.Y);
            
            base.Position(y);
        }


        /// <summary>
        /// Find an item in the list and return it's index.
        /// </summary>
        /// <param name="item">Item to search for.</param>
        /// <returns>Item index.</returns>
        public virtual int ItemToIndex(dynamic item)
        {
            return _items.FindIndex(item);
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

            _arrowLeft.Color = Enabled ? Selected ? UnknownColors.Black : UnknownColors.WhiteSmoke : Color.FromArgb(163, 159, 148);
            _arrowRight.Color = Enabled ? Selected ? UnknownColors.Black : UnknownColors.WhiteSmoke : Color.FromArgb(163, 159, 148);
            float offset = ((_rectangleBackground.Size.Width - _rectangleSlider.Size.Width)/(_items.Count - 1)) * Index;
            _rectangleSlider.Position = new PointF(250 + Offset.X + offset, _rectangleSlider.Position.Y);
            if (Selected)
            {
                _arrowLeft.Draw();
                _arrowRight.Draw();
            }
            else
            {

            }
            _rectangleBackground.Draw();
            _rectangleSlider.Draw();
            _rectangleDivider.Draw();
        }

        internal virtual void SliderChangedTrigger(int newindex)
        {
            OnSliderChanged?.Invoke(this, newindex);
        }

        internal virtual void SliderSelectedTrigger(int newindex)
        {
            OnSliderSelected?.Invoke(this, newindex);
        }
    }
}