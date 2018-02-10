using System;
using System.Drawing;
using Font = CitizenFX.Core.UI.Font;

namespace NativeUI
{
    public class UIMenuDynamicListItem : UIMenuItem, IListItem
    {
        public enum ChangeDirection
        {
            Left,
            Right
        }

        public delegate string DynamicListItemChangeCallback(UIMenuDynamicListItem sender, ChangeDirection direction);

        protected UIResText _itemText;
        protected Sprite _arrowLeft;
        protected Sprite _arrowRight;

        public string CurrentListItem { get; internal set; }
        public DynamicListItemChangeCallback Callback { get; set; }

        /// <summary>
        /// List item with items generated at runtime
        /// </summary>
        /// <param name="text">Label text</param>
        public UIMenuDynamicListItem(string text, string startingItem, DynamicListItemChangeCallback changeCallback) : this(text, null, startingItem, changeCallback)
        {
        }

        /// <summary>
        /// List item with items generated at runtime
        /// </summary>
        /// <param name="text">Label text</param>
        /// <param name="description">Item description</param>
        public UIMenuDynamicListItem(string text, string description, string startingItem, DynamicListItemChangeCallback changeCallback) : base(text, description)
        {
            const int y = 0;
            _arrowLeft = new Sprite("commonmenu", "arrowleft", new PointF(110, 105 + y), new SizeF(30, 30));
            _arrowRight = new Sprite("commonmenu", "arrowright", new PointF(280, 105 + y), new SizeF(30, 30));
            _itemText = new UIResText("", new PointF(290, y + 104), 0.35f, UnknownColors.White, Font.ChaletLondon,
                UIResText.Alignment.Right);

            CurrentListItem = startingItem;
            Callback = changeCallback;
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
        /// Draw item.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            string caption = CurrentListItem;
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
            return CurrentListItem;
        }
    }
}