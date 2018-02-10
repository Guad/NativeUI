using System;

using System.IO;
using System.Reflection;
using CitizenFX.Core.Native;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using System.Drawing;

namespace NativeUI
{
    public class Sprite
    {
        public PointF Position;
        public SizeF Size;
        public Color Color;
        public bool Visible;
        public float Heading;

        public string TextureDict
        {
            get { return _textureDict; }
            set
            {
                _textureDict = value;
                //if(_autoload && !Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, value))
                //Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, value, true);
            }
        }

        public string TextureName;
        private string _textureDict;

        /// <summary>
        /// Creates a game sprite object from a texture dictionary and texture name.
        /// </summary>
        /// <param name="textureDict"></param>
        /// <param name="textureName"></param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="heading"></param>
        /// <param name="color"></param>
        public Sprite(string textureDict, string textureName, PointF position, SizeF size, float heading, Color color) //BASE
        {
            //if (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, textureDict))
            //Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, textureDict, true);
            TextureDict = textureDict;
            TextureName = textureName;

            Position = position;
            Size = size;
            Heading = heading;
            Color = color;
            Visible = true;
        }

        /// <summary>
        /// Creates a game sprite object from a texture dictionary and texture name.
        /// </summary>
        /// <param name="textureDict"></param>
        /// <param name="textureName"></param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        public Sprite(string textureDict, string textureName, PointF position, SizeF size) : this(textureDict, textureName, position, size, 0f, Color.FromArgb(255, 255, 255, 255))
        {
        }


        /// <summary>
        /// Draws the sprite on a 1080-pixels height base.
        /// </summary>
        public void Draw()
        {
            if (!Visible) return;
            if (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, TextureDict))
                Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, TextureDict, true);

            int screenw = Screen.Resolution.Width;
            int screenh = Screen.Resolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;


            float w = (Size.Width / width);
            float h = (Size.Height / height);
            float x = (Position.X / width) + w * 0.5f;
            float y = (Position.Y / height) + h * 0.5f;

            Function.Call(Hash.DRAW_SPRITE, TextureDict, TextureName, x, y, w, h, Heading, Color.R, Color.G, Color.B, Color.A);
        }

        public static void Draw(string dict, string name, int xpos, int ypos, int boxWidth, int boxHeight, float rotation, Color color)
        {
            if (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, dict))
                Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, dict, true);

            int screenw = Screen.Resolution.Width;
            int screenh = Screen.Resolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;


            float w = (boxWidth / width);
            float h = (boxHeight / height);
            float x = (xpos / width) + w * 0.5f;
            float y = (ypos / height) + h * 0.5f;

            Function.Call(Hash.DRAW_SPRITE, dict, name, x, y, w, h, rotation, color.R, color.G, color.B, color.A);
        }

        /*
        /// <summary>
        /// Draw a custom texture from a file on a 1080-pixels height base.
        /// </summary>
        /// <param name="path">Path to texture file.</param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        public static void DrawTexture(string path, PointF position, SizeF size, float rotation, Color color)
        {
            int screenw = Screen.Resolution.Width;
            int screenh = Screen.Resolution.Height;
            
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            float width = height * ratio;
            
            float reduceX = Screen.Width / width;
            float reduceY = Screen.Height / height;

            
            PointF extra = new PointF(0,0);
            if (screenw == 1914 && screenh == 1052) //TODO: Fix this when ScriptHookVDotNet 1.2 comes out.
                extra = new PointF(15, 0);

            DrawTexture(path, 1, 1, 60,
                new PointF(Convert.ToInt32(position.X*reduceX) + extra.X, Convert.ToInt32(position.Y*reduceY) + extra.Y),
                new PointF(0f, 0f), 
                new SizeF(Convert.ToInt32(size.Width * reduceX), Convert.ToInt32(size.Height * reduceY)),
                rotation, color);
        }

        /// <summary>
        /// Draw a custom texture from a file on a 1080-pixels height base.
        /// </summary>
        /// <param name="path">Path to texture file.</param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        public static void DrawTexture(string path, PointF position, SizeF size)
        {
            int screenw = Game.ScreenResolution.Width;
            int screenh = Game.ScreenResolution.Height;

            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            float width = height * ratio;

            float reduceX = UI.WIDTH / width;
            float reduceY = UI.HEIGHT / height;


            PointF extra = new PointF(0, 0);
            if (screenw == 1914 && screenh == 1052) //TODO: Fix this when ScriptHookVDotNet 1.2 comes out.
                extra = new PointF(15, 0);

            UI.DrawTexture(path, 1, 1, 60,
                new PointF(Convert.ToInt32(position.X * reduceX) + extra.X, Convert.ToInt32(position.Y * reduceY) + extra.Y),
                new PointFF(0f, 0f),
                new SizeF(Convert.ToInt32(size.Width * reduceX), Convert.ToInt32(size.Height * reduceY)),
                0f, Color.White);
        }
        */

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