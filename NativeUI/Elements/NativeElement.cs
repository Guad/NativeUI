using GTA;
using NativeUI.Extensions;
using System.Drawing;

namespace NativeUI.Elements
{
    public class NativeElement : UIElement
    {
        #region Private Properties

        /// <summary>
        /// The literal position.
        /// </summary>
        protected Point InternalPosition { get; set; } = Point.Empty;
        /// <summary>
        /// The relative position.
        /// </summary>
        protected PointF RelativePosition { get; set; } = PointF.Empty;
        /// <summary>
        /// The literal size.
        /// </summary>
        protected Size InternalSize { get; set; } = Size.Empty;
        /// <summary>
        /// The relative size.
        /// </summary>
        protected SizeF RelativeSize { get; set; } = SizeF.Empty;

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
            get => InternalPosition;
            set
            {
                InternalPosition = value;
                PointF Output = value.ToRelative(out _, out _);
                float Ratio = (float)Game.ScreenResolution.Width / Game.ScreenResolution.Height;
                float Width = 1080f * Ratio;
                RelativePosition = new PointF((Position.X / Width) + (RelativeSize.Width * 0.5f), (Position.Y / 1080f) + (RelativeSize.Height * 0.5f));
            }
        }
        /// <summary>
        /// Size of the rectangle.
        /// </summary>
        public Size Size
        {
            get => InternalSize;
            set
            {
                InternalSize = value;
                SizeF Output = value.ToRelative(out float Ratio, out _);
                RelativeSize = new SizeF(value.Width / (1080f * Ratio), value.Height / 1080f);
            }
        }
        /// <summary>
        /// If the control is enabled for drawing.
        /// </summary>
        public bool Enabled { get; set; } = true;

        #endregion

        #region Drawing

        public virtual void Draw(Size offset) => throw new System.NotImplementedException();

        public virtual void Draw() => Draw(Size.Empty);

        #endregion
    }
}
