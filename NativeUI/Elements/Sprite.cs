using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using GTA;
using GTA.Native;

namespace NativeUI
{
    /// <summary>
    /// Class for drawing sprites.
    /// </summary>
    public class Sprite
    {
        /// <summary>
        /// The screen position.
        /// </summary>
        private PointF RelativePosition { get; set; }
        /// <summary>
        /// The size of the sprite.
        /// </summary>
        private SizeF RelativeSize { get; set; }
        /// <summary>
        /// The screen position.
        /// </summary>
        private Point InternalPosition { get; set; }
        /// <summary>
        /// The size of the sprite.
        /// </summary>
        private Size InternalSize { get; set; }
        
        /// <summary>
        /// The screen position.
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
        /// The size of the sprite.
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
        /// The color or tint to use on the texture.
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// If the sprite should be drawn during the next game tick.
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// The rotation of the sprite.
        /// </summary>
        public float Heading { get; set; }
        /// <summary>
        /// The texture dictionary to load from.
        /// </summary>
        public string TextureDict { get; set; }
        /// <summary>
        /// The name of the texture inside of the dictionary to use.
        /// </summary>
        public string TextureName { get; set; }

        /// <summary>
        /// Creates a game sprite object from a texture dictionary and texture name.
        /// </summary>
        public Sprite(string dict, string texture, Point position, Size size, float heading, Color color) //BASE
        {
            TextureDict = dict;
            TextureName = texture;
            Position = position;
            Size = size;
            Heading = heading;
            Color = color;
            Visible = true;

            Recalculate();
        }

        /// <summary>
        /// Creates a game sprite object from a texture dictionary and texture name.
        /// </summary>
        public Sprite(string dict, string texture, Point position, Size size) : this(dict, texture, position, size, 0f, Color.White)
        {
        }

        /// <summary>
        /// Recalculates the position (X and Y) and size (width and height) of the sprite.
        /// </summary>
        private void Recalculate()
        {
            // Get the 1080p based screen resolution while maintaining the aspect ratio
            SizeF res = Screen.ResolutionMaintainRatio;

            // Calculate the width, height, x and y positions
            float width = (Size.Width / res.Width);
            float height = (Size.Height / res.Height);
            float x = (Position.X / res.Width) + width * 0.5f;
            float y = (Position.Y / res.Height) + height * 0.5f;

            // Finally, set the values
            RelativePosition = new PointF(x, y);
            RelativeSize = new SizeF(width, height);
        }

        /// <summary>
        /// Draws the sprite on a 1080-pixels height base.
        /// </summary>
        public void Draw()
        {
            // If the sprite should not be visible during this game tick, return
            if (!Visible)
            {
                return;
            }

            // If the texture dictionary has not been loaded, request it
            if (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, TextureDict))
            {
                Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, TextureDict, true);
            }

            // Finally, call DRAW_SPRITE to draw it with the information that we already have
            Function.Call(Hash.DRAW_SPRITE, TextureDict, TextureName, RelativePosition.X, RelativePosition.Y, RelativeSize.Width, RelativeSize.Height, Heading, Color.R, Color.G, Color.B, Color.A);
        }

        [Obsolete("Create a Sprite object and call Draw that instead.", true)]
        public static void Draw(string dict, string texture, int x, int y, int width, int height, float rotation, Color color)
        {
            // Create the sprite object
            Sprite sprite = new Sprite(dict, texture, new Point(x, y), new Size(width, height), rotation, color);
            // And draw said object
            sprite.Draw();
        }

        /// <summary>
        /// Draw a custom texture from a file on a 1080-pixels height base.
        /// </summary>
        /// <param name="path">Path to texture file.</param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        public static void DrawTexture(string path, Point position, Size size, float rotation, Color color)
        {
            int screenw = Game.ScreenResolution.Width;
            int screenh = Game.ScreenResolution.Height;
            
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            float width = height * ratio;
            
            float reduceX = UI.WIDTH / width;
            float reduceY = UI.HEIGHT / height;

            
            Point extra = new Point(0,0);
            if (screenw == 1914 && screenh == 1052) //TODO: Fix this when ScriptHookVDotNet 1.2 comes out.
                extra = new Point(15, 0);

            UI.DrawTexture(path, 1, 1, 60,
                new Point(Convert.ToInt32(position.X*reduceX) + extra.X, Convert.ToInt32(position.Y*reduceY) + extra.Y),
                new PointF(0f, 0f), 
                new Size(Convert.ToInt32(size.Width * reduceX), Convert.ToInt32(size.Height * reduceY)),
                rotation, color);
        }

        /// <summary>
        /// Draw a custom texture from a file on a 1080-pixels height base.
        /// </summary>
        /// <param name="path">Path to texture file.</param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        public static void DrawTexture(string path, Point position, Size size)
        {
            int screenw = Game.ScreenResolution.Width;
            int screenh = Game.ScreenResolution.Height;

            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            float width = height * ratio;

            float reduceX = UI.WIDTH / width;
            float reduceY = UI.HEIGHT / height;


            Point extra = new Point(0, 0);
            if (screenw == 1914 && screenh == 1052) //TODO: Fix this when ScriptHookVDotNet 1.2 comes out.
                extra = new Point(15, 0);

            UI.DrawTexture(path, 1, 1, 60,
                new Point(Convert.ToInt32(position.X * reduceX) + extra.X, Convert.ToInt32(position.Y * reduceY) + extra.Y),
                new PointF(0f, 0f),
                new Size(Convert.ToInt32(size.Width * reduceX), Convert.ToInt32(size.Height * reduceY)),
                0f, Color.White);
        }


        /// <summary>
        /// Save an embedded resource to a temporary file.
        /// </summary>
        /// <param name="yourAssembly">Your executing assembly.</param>
        /// <param name="fullResourceName">Resource name including your solution name. E.G MyMenuMod.banner.png</param>
        /// <returns>Absolute path to the written file.</returns>
        public static string WriteFileFromResources(Assembly yourAssembly, string fullResourceName)
        {
            string tmpPath = Path.GetTempFileName();
            return WriteFileFromResources(yourAssembly, fullResourceName, tmpPath);
        }


        /// <summary>
        /// Save an embedded resource to a concrete path.
        /// </summary>
        /// <param name="yourAssembly">Your executing assembly.</param>
        /// <param name="fullResourceName">Resource name including your solution name. E.G MyMenuMod.banner.png</param>
        /// <param name="savePath">Path to where save the file, including the filename.</param>
        /// <returns>Absolute path to the written file.</returns>
        public static string WriteFileFromResources(Assembly yourAssembly, string fullResourceName, string savePath)
        {
            using (Stream stream = yourAssembly.GetManifestResourceStream(fullResourceName))
            {
                if (stream != null)
                {
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, Convert.ToInt32(stream.Length));

                    using (FileStream fileStream = File.Create(savePath))
                    {
                        fileStream.Write(buffer, 0, Convert.ToInt32(stream.Length));
                        fileStream.Close();
                    }
                }
            }
            return Path.GetFullPath(savePath);
        }
    }
}
