using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace NativeUI
{
    public class Sprite
    {
        public PointF Position; //TURNING Point TO PointF (same for Size to SizeF) DOESN'T BREAK COMPATIBILITY AND ADDS PRECISION
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
		public async Task Draw()
		{
			if (!Visible) return;
			if (!API.HasStreamedTextureDictLoaded(TextureDict))
				API.RequestStreamedTextureDict(TextureDict, true);

			int screenw = CitizenFX.Core.UI.Screen.Resolution.Width;
            int screenh = CitizenFX.Core.UI.Screen.Resolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw/screenh;
            var width = height*ratio;


            float w = (Size.Width / width);
            float h = (Size.Height / height);
            float x = (Position.X / width) + w * 0.5f;
            float y = (Position.Y / height) + h * 0.5f;

			API.DrawSprite(TextureDict, TextureName, x, y, w, h, Heading, Color.R, Color.G, Color.B, Color.A);
		}

		public static void Draw(string dict, string name, int xpos, int ypos, int boxWidth, int boxHeight, float rotation, Color color)
        {
			if (!API.HasStreamedTextureDictLoaded(dict))
				API.RequestStreamedTextureDict(dict, true);


			int screenw = CitizenFX.Core.UI.Screen.Resolution.Width;
            int screenh = CitizenFX.Core.UI.Screen.Resolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;


            float w = (boxWidth / width);
            float h = (boxHeight / height);
            float x = (xpos / width) + w * 0.5f;
            float y = (ypos / height) + h * 0.5f;

			API.DrawSprite(dict, name, x, y, w, h, rotation, color.R, color.G, color.B, color.A);
		}

		/// <summary>
		/// Draw a custom texture from a file on a 1080-pixels height base.
		/// </summary>
		/// <param name="path">Path to texture file.</param>
		/// <param name="position"></param>
		/// <param name="size"></param>
		public static void DrawTexture(string path, Point position, Size size, float rotation, Color color)
        {
            throw new NotSupportedException("Custom PNG textures are not supported in FiveM.");
        }

        /// <summary>
        /// Draw a custom texture from a file on a 1080-pixels height base.
        /// </summary>
        /// <param name="path">Path to texture file.</param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        public static void DrawTexture(string path, Point position, Size size)
        {
            throw new NotSupportedException("Custom PNG textures are not supported in FiveM.");
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