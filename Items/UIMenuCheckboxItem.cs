using System;
using System.Drawing;

namespace NativeUI
{
    public class UIMenuCheckboxItem : UIMenuItem
    {
        protected Sprite _checkedSprite;

        /// <summary>
        /// Triggered when the checkbox state is changed.
        /// </summary>
        public event ItemCheckboxEvent CheckboxEvent;

        /// <summary>
        /// Checkbox item with a toggleable checkbox.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="check">Boolean value whether the checkbox is checked.</param>
        public UIMenuCheckboxItem(string text, bool check)
            : this(text, check, "")
        {
        }

        /// <summary>
        /// Checkbox item with a toggleable checkbox.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="check">Boolean value whether the checkbox is checked.</param>
        /// <param name="description">Description for this item.</param>
        public UIMenuCheckboxItem(string text, bool check, string description)
            : base(text, description)
        {
            const int y = 0;
            _checkedSprite = new Sprite("commonmenu", "shop_box_blank", new PointF(410, y + 95), new SizeF(50, 50));
            Checked = check;
        }


        /// <summary>
        /// Change or get whether the checkbox is checked.
        /// </summary>
        public bool Checked { get; set; }


        /// <summary>
        /// Change item's position.
        /// </summary>
        /// <param name="y">New Y value.</param>
        public override void Position(int y)
        {
            base.Position(y);
            _checkedSprite.Position = new PointF(380 + Offset.X + Parent.WidthOffset, y + 138 + Offset.Y);
        }


        /// <summary>
        /// Draw item.
        /// </summary>
        public override void Draw()
        {
            base.Draw();
            _checkedSprite.Position = new PointF(380 + Offset.X + Parent.WidthOffset, _checkedSprite.Position.Y);
            if (Selected)
            {
                _checkedSprite.TextureName = Checked ? "shop_box_tickb" : "shop_box_blankb";
            }
            else
            {
                _checkedSprite.TextureName = Checked ? "shop_box_tick" : "shop_box_blank";
            }
            _checkedSprite.Draw();
        }

        public void CheckboxEventTrigger()
        {
            CheckboxEvent?.Invoke(this, Checked);
        }

        public override void SetRightBadge(BadgeStyle badge)
        {
            throw new Exception("UIMenuCheckboxItem cannot have a right badge.");
        }

        public override void SetRightLabel(string text)
        {
            throw new Exception("UIMenuListItem cannot have a right label.");
        }
    }
}