using System.Drawing;
using GTA;

namespace NativeUI
{
    public class UIMenuItem
    {
        private readonly UIRectangle _rectangle;
        private readonly UIText _text;
        private bool _selected;
        private Sprite _selectedSprite;

        public UIMenuItem(string text, int y)
        {
            _rectangle = new UIRectangle(new Point(0, y + 90), new Size(300, 25), Color.FromArgb(100, 0, 0, 0));
            _text = new UIText(text, new Point(10, y + 93), 0.3f, Color.WhiteSmoke, GTA.Font.ChaletLondon, false);

            _selectedSprite = new Sprite("commonmenu", "gradient_nav", new Point(0, y + 90), new Size(300, 25));
        }

        public virtual bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                //_rectangle.Color = value ? Color.FromArgb(250, 255, 255, 255) : Color.FromArgb(100, 0, 0, 0);
                _text.Color = value ? Color.Black : Color.WhiteSmoke;
            }
        }

        public virtual void Position(int y)
        {
            _rectangle.Position = new Point(0 + Offset.X, y + 90 + Offset.Y);
            _selectedSprite.Position = new Point(0 + Offset.X, y + 90 + Offset.Y);
            _text.Position = new Point(10 + Offset.X, y + 93 + Offset.Y);
        }

        public virtual void Draw()
        {
            _rectangle.Draw();
            _text.Draw();
            if (Selected)
                _selectedSprite.Draw();
        }

        public Point Offset { get; set; }
    }
}
