using System.Drawing;

namespace NativeUI
{
    public class UIMenuCheckboxItem : UIMenuItem
    {
        private Sprite _checkedSprite;

        public UIMenuCheckboxItem(string text, int y, bool check)
            : base(text, y)
        {
            _checkedSprite = new Sprite("commonmenu", "shop_box_blank", new Point(260, y + 85), new Size(35, 35));
            Checked = check;
        }

        public bool Checked { get; set; }

        public override void Position(int y)
        {
            base.Position(y);
            _checkedSprite.Position = new Point(260 + Offset.X, y + 85 + Offset.Y);
        }

        public override void Draw()
        {
            base.Draw();
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
    }
}