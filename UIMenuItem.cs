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

        /// <summary>
        /// Basic menu button.
        /// </summary>
        /// <param name="text">Button label.</param>
        public UIMenuItem(string text) : this(text, "")
        {
        }

        /// <summary>
        /// Basic menu button.
        /// </summary>
        /// <param name="text">Button label.</param>
        /// <param name="description">Description.</param>
        public UIMenuItem(string text, string description)
        {
            int y = 0;
            Text = text;
            _rectangle = new UIRectangle(new Point(0, y + 100), new Size(290, 25), Color.FromArgb(150, 0, 0, 0));
            _text = new UIText(text, new Point(5, y + 103), 0.33f, Color.WhiteSmoke, GTA.Font.ChaletLondon, false);
            Description = description;
            _selectedSprite = new Sprite("commonmenu", "gradient_nav", new Point(0, y + 100), new Size(290, 25));
        }


        /// <summary>
        /// Whether this item is currently selected.
        /// </summary>
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

        public virtual string Description { get; set; }


        /// <summary>
        /// Set item's position.
        /// </summary>
        /// <param name="y"></param>
        public virtual void Position(int y)
        {
            _rectangle.Position = new Point(0 + Offset.X, y + 100 + Offset.Y);
            _selectedSprite.Position = new Point(0 + Offset.X, y + 100 + Offset.Y);
            _text.Position = new Point(5 + Offset.X, y + 103 + Offset.Y);
        }


        /// <summary>
        /// Draw this item.
        /// </summary>
        public virtual void Draw()
        {
            _rectangle.Draw();
            _text.Draw();
            if (Selected)
                _selectedSprite.Draw();
        }

        public Point Offset { get; set; }
        public string Text { get; set; }
    }
}
