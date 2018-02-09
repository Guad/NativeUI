using System.Drawing;

namespace NativeUI
{
    public class UIMenuColoredItem : UIMenuItem
    {
        public Color MainColor { get; set; }
        public Color HighlightColor { get; set; }

        public Color TextColor { get; set; }
        public Color HighlightedTextColor { get; set; }

        public UIMenuColoredItem(string label, Color color, Color highlightColor) : base(label)
        {
            MainColor = color;
            HighlightColor = highlightColor;

            TextColor = UnknownColors.White;
            HighlightedTextColor = UnknownColors.Black;

            Init();
        }

        public UIMenuColoredItem(string label, string description, Color color, Color highlightColor) : base(label, description)
        {
            MainColor = color;
            HighlightColor = highlightColor;

            TextColor = UnknownColors.White;
            HighlightedTextColor = UnknownColors.Black;

            Init();
        }

        protected void Init()
        {
            _selectedSprite = new Sprite("commonmenu", "gradient_nav", new PointF(0, 0), new SizeF(431, 38), 0, HighlightColor);
            _rectangle = new UIResRectangle(new PointF(0, 0), new SizeF(431, 38), Color.FromArgb(150, 0, 0, 0));
            _text = new UIResText(Text, new PointF(8, 0), 0.33f, UnknownColors.WhiteSmoke, CitizenFX.Core.UI.Font.ChaletLondon, UIResText.Alignment.Left);
            Description = Description;

            _badgeLeft = new Sprite("commonmenu", "", new PointF(0, 0), new SizeF(40, 40));
            _badgeRight = new Sprite("commonmenu", "", new PointF(0, 0), new SizeF(40, 40));

            _labelText = new UIResText("", new PointF(0, 0), 0.35f) { TextAlignment = UIResText.Alignment.Right };
        }


        public override void Draw()
        {
            _rectangle.Size = new SizeF(431 + Parent.WidthOffset, 38);
            _selectedSprite.Size = new SizeF(431 + Parent.WidthOffset, 38);

            if (Hovered && !Selected)
            {
                _rectangle.Color = Color.FromArgb(20, 255, 255, 255);
                _rectangle.Draw();
            }
            if (Selected)
            {
                _selectedSprite.Color = HighlightColor;
                _selectedSprite.Draw();
            }
            else
            {
                _selectedSprite.Color = MainColor;
                _selectedSprite.Draw();
            }

            _text.Color = Enabled ? Selected ? HighlightedTextColor : TextColor : Color.FromArgb(163, 159, 148);

            if (LeftBadge != BadgeStyle.None)
            {
                _text.Position = new PointF(35 + Offset.X, _text.Position.Y);
                _badgeLeft.TextureDict = BadgeToSpriteLib(LeftBadge);
                _badgeLeft.TextureName = BadgeToSpriteName(LeftBadge, Selected);
                _badgeLeft.Color = BadgeToColor(LeftBadge, Selected);
                _badgeLeft.Draw();
            }
            else
            {
                _text.Position = new PointF(8 + Offset.X, _text.Position.Y);
            }

            if (RightBadge != BadgeStyle.None)
            {
                _badgeRight.Position = new PointF(385 + Offset.X + Parent.WidthOffset, _badgeRight.Position.Y);
                _badgeRight.TextureDict = BadgeToSpriteLib(RightBadge);
                _badgeRight.TextureName = BadgeToSpriteName(RightBadge, Selected);
                _badgeRight.Color = BadgeToColor(RightBadge, Selected);
                _badgeRight.Draw();
            }

            if (!string.IsNullOrWhiteSpace(RightLabel))
            {
                _labelText.Position = new PointF(420 + Offset.X + Parent.WidthOffset, _labelText.Position.Y);
                _labelText.Caption = RightLabel;
                _labelText.Color = _text.Color = Enabled ? Selected ? HighlightedTextColor : TextColor : Color.FromArgb(163, 159, 148);
                _labelText.Draw();
            }
            _text.Draw();
        }
    }
}