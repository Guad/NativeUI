using GTA;
using System.Drawing;

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
        /// Our internal caption.
        /// </summary>
        private string _Caption = "";

        #endregion

        #region Public Properties

        /// <summary>
        /// If the text is enabled or not.
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// The UTF-8 string to draw on the screen.
        /// </summary>
        public string Caption
        {
            get
            {
                return _Caption;
            }
            set
            {
                _Caption = value;
            }
        }
        /// <summary>
        /// The position of the text
        /// </summary>
        public Point Position { get; set; }
        /// <summary>
        /// The color of the text.
        /// </summary>
        public Color Color { get; set; }

        #endregion

        #region Constructors

        public NativeText(string caption)
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

        }

        #endregion
    }
}
