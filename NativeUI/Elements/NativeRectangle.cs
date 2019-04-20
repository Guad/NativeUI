using System.Drawing;
using GTA;
using GTA.Native;
using NativeUI.Extensions;

namespace NativeUI.Elements
{
    public class NativeRectangle : UIElement
    {
        #region Private Properties

        /// <summary>
        /// The literal position.
        /// </summary>
        private Point _Position { get; set; } = Point.Empty;
        /// <summary>
        /// The relative position.
        /// </summary>
        private PointF _RelativePos { get; set; } = PointF.Empty;
        /// <summary>
        /// The literal size.
        /// </summary>
        private Size _Size { get; set; } = Size.Empty;
        /// <summary>
        /// The relative size.
        /// </summary>
        private SizeF _RelativeSize { get; set; } = SizeF.Empty;

        #endregion

        #region Public Properties

        /// <summary>
        /// Color of the rectangle.
        /// </summary>
        public Color Color { get; set; } = Color.White;
        /// <summary>
        /// Position of the rectangle.
        /// </summary>
        public Point Position
        {
            get => _Position;
            set
            {
                _Position = value;
                PointF Output = value.ToRelative(out _, out _);
                float Ratio = (float)Game.ScreenResolution.Width / Game.ScreenResolution.Height;
                float Width = 1080f * Ratio;
                _RelativePos = new PointF((Position.X / Width) + (_RelativeSize.Width * 0.5f), (Position.Y / 1080f) + (_RelativeSize.Height * 0.5f));
            }
        }
        /// <summary>
        /// Size of the rectangle.
        /// </summary>
        public Size Size
        {
            get => _Size;
            set
            {
                _Size = value;
                SizeF Output = value.ToRelative(out float Ratio, out _);
                _RelativeSize = new SizeF(value.Width / (1080f * Ratio), value.Height / 1080f);
            }
        }
        /// <summary>
        /// If the control is enabled for drawing.
        /// </summary>
        public bool Enabled { get; set; } = true;

        #endregion

        #region Constructors

        public NativeRectangle(Point position, Size size) : this(position, size, Color.White) { }

        public NativeRectangle(Point position, Size size, Color color)
        {
            Position = position;
            Size = size;
            Color = color;
        }

        #endregion

        #region Drawing

        /// <summary>
        /// Draws the rectangle on screen.
        /// </summary>
        public void Draw() => Draw(Size.Empty);

        /// <summary>
        /// Draws the rectangle on screen with the specified offset.
        /// </summary>
        public void Draw(Size offset)
        {
            // For now, the calculations at the top do not work and need tweaking
            // Calculate the width of the control and make the X and Y position
            float Width = 1080f * ((float)Game.ScreenResolution.Width / Game.ScreenResolution.Height);
            float X = (Position.X / Width) + (_RelativeSize.Width * 0.5f);
            float Y = (Position.Y / 1080f) + (_RelativeSize.Height * 0.5f);

            // Finally, draw the rectangle
            Function.Call(Hash.DRAW_RECT, X, Y, _RelativeSize.Width, _RelativeSize.Height, Color.R, Color.G, Color.B, Color.A);
        }

        #endregion
    }
}
