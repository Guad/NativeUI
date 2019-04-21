using System.Drawing;
using GTA.Native;

namespace NativeUI.Elements
{
    public class NativeRectangle : NativeElement
    {
        #region Constructors

        public NativeRectangle(Point position, Size size) : this(position, size, Color.White) { }

        public NativeRectangle(Point position, Size size, Color color)
        {
            Size = size;
            Position = position;
            Color = color;
        }

        #endregion

        #region Drawing

        /// <summary>
        /// Draws the rectangle on screen.
        /// </summary>
        public override void Draw()
        {
            // Just draw the rectangle
            Function.Call(Hash.DRAW_RECT, RelativePosition.X, RelativePosition.Y, RelativeSize.Width, RelativeSize.Height, Color.R, Color.G, Color.B, Color.A);
        }

        #endregion
    }
}
