using System.Drawing;
using GTA;
using GTA.Native;
using NativeUI.Extensions;

namespace NativeUI.Elements
{
    public class NativeSprite : UIElement
    {
        #region Private Properties

        /// <summary>
        /// The literal position of the sprite.
        /// </summary>
        private Point _Position { get; set; } = Point.Empty;
        /// <summary>
        /// The relative position of the sprite.
        /// </summary>
        private PointF _RelativePos { get; set; } = PointF.Empty;
        /// <summary>
        /// The literal position of the sprite.
        /// </summary>
        private Size _Size { get; set; } = Size.Empty;
        /// <summary>
        /// The relative position of the sprite.
        /// </summary>
        private SizeF _RelativeSize { get; set; } = SizeF.Empty;

        #endregion

        #region Public Properties

        /// <summary>
        /// The color of the sprite.
        /// </summary>
        public Color Color { get; set; } = Color.White;
        /// <summary>
        /// The literal position of the sprite.
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
        /// The literal size of the sprite.
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
        /// If the sprite is enabled for drawing.
        /// </summary>
        public bool Enabled { get; set; } = true;
        /// <summary>
        /// The heading of the sprite.
        /// </summary>
        public float Heading { get; set; } = 0;
        /// <summary>
        /// The texture dictionary of the sprite.
        /// </summary>
        public string TextureDict { get; set; } = "";
        /// <summary>
        /// The texture name of the sprite.
        /// </summary>
        public string TextureName { get; set; } = "";

        #endregion

        #region Constructors

        public NativeSprite(string dict, string name) : this(dict, name, Point.Empty, Size.Empty, 0, Color.White) { }

        public NativeSprite(string dict, string name, Point position) : this(dict, name, position, Size.Empty, 0, Color.White) { }

        public NativeSprite(string dict, string name, Point position, Size size) : this(dict, name, position, size, 0, Color.White) { }

        public NativeSprite(string dict, string name, Point position, Size size, float heading) : this(dict, name, position, size, heading, Color.White) { }

        public NativeSprite(string dict, string name, Point position, Size size, float heading, Color color)
        {
            // Keep Size and Position in that order!
            TextureDict = dict;
            TextureName = name;
            Heading = heading;
            Color = color;
            Size = size;
            Position = position;
        }

        #endregion

        #region Drawing

        /// <summary>
        /// Draws the sprite on screen.
        /// </summary>
        public void Draw() => Draw(Size.Empty);

        /// <summary>
        /// Draws the sprite on screen with the specified offset.
        /// </summary>
        public void Draw(Size offset)
        {
            // If the texture dictionary has not been loaded
            if (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, TextureDict))
            {
                // Request it
                Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, TextureDict, true);
            }
            
            // For now, the calculations at the top do not work and need tweaking
            // Calculate the width of the control and make the X and Y position
            float Width = 1080f * ((float)Game.ScreenResolution.Width / Game.ScreenResolution.Height);
            float X = (Position.X / Width) + (_RelativeSize.Width * 0.5f);
            float Y = (Position.Y / 1080f) + (_RelativeSize.Height * 0.5f);

            // Finally, draw the sprite
            Function.Call(Hash.DRAW_SPRITE, TextureDict, TextureName, X, Y, _RelativeSize.Width, _RelativeSize.Height, Heading, Color.R, Color.G, Color.B, Color.A);
        }

        #endregion
    }
}
