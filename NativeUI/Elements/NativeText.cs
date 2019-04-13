using GTA;
using GTA.Native;
using System;
using System.Drawing;
using System.Text;

namespace NativeUI.Elements
{
    /// <summary>
    /// The alignment of the text to draw.
    /// </summary>
    public enum TextAlignment
    {
        Left,
        Centered,
        Right,
    }

    /// <summary>
    /// New Design for the NativeUI texts.
    /// </summary>
    public class NativeText : UIElement
    {
        #region Private Properties

        /// <summary>
        /// Internal absolute X-Y position.
        /// </summary>
        private Point _Position = Point.Empty;
        /// <summary>
        /// Internal relative X-Y position.
        /// </summary>
        private PointF _Relative = PointF.Empty;
        /// <summary>
        /// The raw word wrap value.
        /// </summary>
        private float _WordWrap = 0f;
        /// <summary>
        /// The already calculated word wrapping.
        /// </summary>
        private float CalculatedWrap = 0f;
        /// <summary>
        /// Internal width calculation.
        /// </summary>
        private float Width = 0f;

        #endregion

        #region Public Properties

        /// <summary>
        /// If the text is enabled or not.
        /// </summary>
        public bool Enabled { get; set; } = true;
        /// <summary>
        /// The UTF-8 string to draw on the screen.
        /// </summary>
        public string Caption { get; set; } = "";
        /// <summary>
        /// The position of the text
        /// </summary>
        public Point Position
        {
            get => _Position;
            set
            {
                _Position = value;
                float W = Game.ScreenResolution.Width;
                float H = Game.ScreenResolution.Height;
                float Ratio = W / H;
                Width = 1080f * Ratio;
                _Relative = new PointF(Position.X / Width, Position.Y / 1080f);
            }
        }
        /// <summary>
        /// The color of the text.
        /// </summary>
        public Color Color { get; set; } = Color.White;
        /// <summary>
        /// The Internal GTA Font to be used.
        /// </summary>
        public GTA.Font Font { get; set; } = GTA.Font.ChaletLondon;
        /// <summary>
        /// The scale of the text between 0.5f and 3.0f.
        /// </summary>
        public float Scale { get; set; } = 1f;
        /// <summary>
        /// If the text should be drawn with a shadow.
        /// </summary>
        public bool Shadow { get; set; } = false;
        /// <summary>
        /// If the text should have an outline.
        /// </summary>
        public bool Outline { get; set; } = false;
        /// <summary>
        /// The alignment of the text.
        /// </summary>
        public TextAlignment Alignment { get; set; } = TextAlignment.Left;
        /// <summary>
        /// The maximum width for word wrapping.
        /// Set to zero to disable.
        /// </summary>
        public float WordWrap
        {
            get
            {
                return _WordWrap;
            }
            set
            {
                _WordWrap = value;
                CalculatedWrap = (Position.X + value) / Width;
            }
        }

        #endregion

        #region Constructors

        public NativeText(string caption, Point position)
        {
            // Start by storing our properties
            Caption = caption;
        }

        #endregion

        #region Drawing

        /// <summary>
        /// Draws the text on screen.
        /// </summary>
        public void Draw() => Draw(Size.Empty);
        
        /// <summary>
        /// Draws the text on screen with the specified offset.
        /// </summary>
        public void Draw(Size offset)
        {
            // Set some basics
            Function.Call(Hash.SET_TEXT_FONT, (int)Font);
            Function.Call(Hash.SET_TEXT_SCALE, 1.0f, Scale);
            Function.Call(Hash.SET_TEXT_COLOUR, Color.R, Color.G, Color.B, Color.A);
            if (Shadow) Function.Call(Hash.SET_TEXT_DROP_SHADOW);
            if (Outline) Function.Call(Hash.SET_TEXT_OUTLINE);

            // Change the alignment to the correct value
            switch (Alignment)
            {
                case TextAlignment.Centered:
                    Function.Call(Hash.SET_TEXT_CENTRE, true);
                    break;
                case TextAlignment.Right:
                    Function.Call(Hash.SET_TEXT_RIGHT_JUSTIFY, true);
                    Function.Call(Hash.SET_TEXT_WRAP, 0, _Relative.X);
                    break;
            }

            // Set the correct word wrap acording to our calculations
            if (WordWrap != 0)
            {
                Function.Call(Hash.SET_TEXT_WRAP, _Relative.X, CalculatedWrap);
            }

            // Start adding the text
            Function.Call(Hash._SET_TEXT_ENTRY, "jamyfafi");
            Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, Caption);
            // And draw it
            Function.Call(Hash._DRAW_TEXT, _Relative.X, _Relative.Y);
        }

        #endregion
    }
}
