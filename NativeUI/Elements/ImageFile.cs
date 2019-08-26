using System.Drawing;
using GTA;

namespace NativeUI.Elements
{
    /// <summary>
    /// Class used for drawing PNG files.
    /// </summary>
    public class ImageFile : INativeElement
    {
        /// <summary>
        /// The screen position.
        /// </summary>
        private Point LiteralPosition { get; set; }
        /// <summary>
        /// The size of the sprite.
        /// </summary>
        private Size LiteralSize { get; set; }
        /// <summary>
        /// The screen position.
        /// </summary>
        private Point InternalPosition { get; set; }
        /// <summary>
        /// The size of the sprite.
        /// </summary>
        private Size InternalSize { get; set; }

        /// <summary>
        /// If the image should be drawn during the next game tick.
        /// </summary>
        public bool Enabled { get; set; } = true;
        /// <summary>
        /// The position of the texture.
        /// </summary>
        public Point Position
        {
            get => InternalPosition;
            set
            {
                InternalPosition = value;
                Recalculate();
            }
        }
        /// <summary>
        /// The size of the texture.
        /// </summary>
        public Size Size
        {
            get => InternalSize;
            set
            {
                InternalSize = value;
                Recalculate();
            }
        }
        /// <summary>
        /// The color or tint of the image.
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// The rotation or heading of the PNG file.
        /// </summary>
        public float Rotation { get; set; }
        /// <summary>
        /// The PNG file that we are going to draw.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Creates a texture object that loads the specified PNG file.
        /// </summary>
        public ImageFile(string file, Point position, Size size) : this(file, position, size, 0, Color.White)
        {
        }

        /// <summary>
        /// Creates a texture object that loads the specified PNG file.
        /// </summary>
        public ImageFile(string file, Point position, Size size, float rotation, Color color)
        {
            // Save our parameters
            Position = position;
            Size = size;
            Color = color;
            Rotation = rotation;
            Filename = file;
        }

        /// <summary>
        /// Recalculates the position (X and Y) and size (width and height) of the image.
        /// </summary>
        public void Recalculate()
        {
            // Get the 1080p based screen resolution while maintaining the aspect ratio
            SizeF res = Screen.ResolutionMaintainRatio;
            
            // Calculate the reduced values
            float reduceX = UI.WIDTH / res.Width;
            float reduceY = UI.HEIGHT / res.Height;

            // And set the correct values
            LiteralPosition = new Point((int)(Position.X * reduceX), (int)(Position.Y * reduceY));
            LiteralSize = new Size((int)(Size.Width * reduceX), (int)(Size.Height * reduceY));
        }

        /// <summary>
        /// Draws the png file on the set position.
        /// </summary>
        public void Draw() => Draw(Size.Empty);
        /// <summary>
        /// Draws the png file on the set position.
        /// </summary>
        public void Draw(Size offset)
        {
            UI.DrawTexture(Filename, 1, 1, 60, LiteralPosition, PointF.Empty, LiteralSize, Rotation, Color);
        }
    }
}
