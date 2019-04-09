using GTA;
using System.Drawing;

namespace NativeUI.Elements
{
    /// <summary>
    /// New Design for the NativeUI texts.
    /// </summary>
    public class NativeText : UIElement
    {
        /// <summary>
        /// If the text is enabled or not.
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// The position of the text
        /// </summary>
        public Point Position { get; set; }
        /// <summary>
        /// The color of the text.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Draws the text on screen.
        /// </summary>
        public void Draw()
        {

        }

        /// <summary>
        /// Draws the text on screen with the specified offset.
        /// </summary>
        public void Draw(Size offset) => Draw();
    }
}
