using System.Drawing;
using GTA;
using GTA.Native;

namespace NativeUI.Elements
{
    public interface ISprite : UIElement { }

    public class NativeSprite : NativeElement, ISprite
    {
        #region Public Properties

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
        public override void Draw()
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
            float X = (Position.X / Width) + (RelativeSize.Width * 0.5f);
            float Y = (Position.Y / 1080f) + (RelativeSize.Height * 0.5f);

            // Finally, draw the sprite
            Function.Call(Hash.DRAW_SPRITE, TextureDict, TextureName, X, Y, RelativeSize.Width, RelativeSize.Height, Heading, Color.R, Color.G, Color.B, Color.A);
        }

        #endregion
    }
}
