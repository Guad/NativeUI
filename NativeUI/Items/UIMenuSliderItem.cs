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
        
        protected int _value = 0;
        protected int _max = 100;
        protected int _multiplier = 5;


        /// <summary>
        /// Triggered when the slider is changed.
        /// </summary>
        public event ItemSliderEvent OnSliderChanged;


        /// <summary>
        /// The maximum value of the slider.
        /// </summary>
        public int Maximum
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
                if (_value > value)
                {
                    _value = value;
                }
            }
        }
        /// <summary>
        /// Curent value of the slider.
        /// </summary>
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value > _max)
                    _value = _max;
                else if (value < 0)
                    _value = 0;
                else
                    _value = value;
                SliderChanged();
            }
        }
        /// <summary>
        /// The multiplier of the left and right navigation movements.
        /// </summary>
        public int Multiplier
        {
            get
            {
                return _multiplier;
            }
            set
            {
                _multiplier = value;
            }
        }


        /// <summary>
        /// List item, with slider.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="items">List that contains your items.</param>
        /// <param name="index">Index in the list. If unsure user 0.</param>
        public UIMenuSliderItem(string text) : this(text, "", false)
        {
        }

        /// <summary>
        /// List item, with slider.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="items">List that contains your items.</param>
        /// <param name="index">Index in the list. If unsure user 0.</param>
        /// <param name="description">Description for this item.</param>
        public UIMenuSliderItem(string text, string description) : this(text, description, false)
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
        public UIMenuSliderItem(string text, string description, bool divider) : base(text, description)
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
        /// Draw item.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            _arrowLeft.Color = Enabled ? Selected ? Color.Black : Color.WhiteSmoke : Color.FromArgb(163, 159, 148);
            _arrowRight.Color = Enabled ? Selected ? Color.Black : Color.WhiteSmoke : Color.FromArgb(163, 159, 148);
            int offset = 176 + Offset.X + _rectangleBackground.Size.Width - _rectangleSlider.Size.Width;
            _rectangleSlider.Position = new Point((int)(offset + (_value / (float)_max * 73)), _rectangleSlider.Position.Y);
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

        internal virtual void SliderChanged()
        {
            OnSliderChanged?.Invoke(this, Value);
        }
    }
}
