using System.Collections.Generic;
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
        public UIMenuSliderItem(string text, int index) : this(text, index, "", false)
        {
        }

        /// <summary>
        /// List item, with slider.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="items">List that contains your items.</param>
        /// <param name="index">Index in the list. If unsure user 0.</param>
        /// <param name="description">Description for this item.</param>
        public UIMenuSliderItem(string text, int index, string description) : this(text, index, description, false)
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
        public UIMenuSliderItem(string text, int index, string description, bool divider)
            : base(text, description)
        {
            const int y = 0;
            _arrowLeft = new Sprite("commonmenutu", "arrowleft", new Point(0, 105 + y), new Size(15, 15));
            _arrowRight = new Sprite("commonmenutu", "arrowright", new Point(0, 105 + y), new Size(15, 15));
            _rectangleBackground = new UIResRectangle(new Point(0, 0), new Size(150, 9), Color.FromArgb(255, 4, 32, 57));
            _rectangleSlider = new UIResRectangle(new Point(0, 0), new Size(75, 9), Color.FromArgb(255, 57, 116, 200));
            if (divider)
            {
                _rectangleDivider = new UIResRectangle(new Point(0, 0), new Size(2, 20), Color.WhiteSmoke);
            }
            else
            {
                _rectangleDivider = new UIResRectangle(new Point(0, 0), new Size(2, 20), Color.Transparent);
            }
            _index = index;
        }

        /// <summary>
        /// Change item's position.
        /// </summary>
        /// <param name="y">New Y position.</param>
        public override void Position(int y)
        {
            _rectangleBackground.Position = new Point(250 + Offset.X, y + 158 + Offset.Y);
            _rectangleSlider.Position = new Point(250 + Offset.X, y + 158 + Offset.Y);
            _rectangleDivider.Position = new Point(323 + Offset.X, y + 153 + Offset.Y);
            _arrowLeft.Position = new Point(235 + Offset.X + Parent.WidthOffset, 155 + y + Offset.Y);
            _arrowRight.Position = new Point(400 + Offset.X + Parent.WidthOffset, 155 + y + Offset.Y);
            
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

            _arrowLeft.Color = Enabled ? Selected ? Color.Black : Color.WhiteSmoke : Color.FromArgb(163, 159, 148);
            _arrowRight.Color = Enabled ? Selected ? Color.Black : Color.WhiteSmoke : Color.FromArgb(163, 159, 148);
            int offset = ((_rectangleBackground.Size.Width - _rectangleSlider.Size.Width)/(_items.Count - 1)) * Index;
            _rectangleSlider.Position = new Point(250 + Offset.X + offset, _rectangleSlider.Position.Y);
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
