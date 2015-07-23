using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using GTA;
using GTA.Native;

namespace NativeUI
{
    public class Sprite
    {
        public Point Position;
        public Size Size;
        public Color Color;
        public bool Visible;
        public float Heading;

        public string TextureDict
        {
            get { return _textureDict; }
            set
            {
                _textureDict = value;
                if(!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, value))
                    Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, value, true);
            }
        }

        public string TextureName;
        private string _textureDict;

        public Sprite(string textureDict, string textureName, Point position, Size size, float heading, Color color) //BASE
        {
            if (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, textureDict))
                Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, textureDict, true);
            TextureDict = textureDict;
            TextureName = textureName;

            Position = position;
            Size = size;
            Heading = heading;
            Color = color;
            Visible = true;
        }

        public Sprite(string textureDict, string textureName, Point position, Size size)
        {
            if (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, textureDict))
                Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, textureDict, true);
            TextureDict = textureDict;
            TextureName = textureName;

            Position = position;
            Size = size;
            Heading = 0f;
            Color = Color.FromArgb(255, 255, 255, 255);
            Visible = true;
        }

        public void Draw()
        {
            if (!Visible) return;
            int screenw = Game.ScreenResolution.Width;
            int screenh = Game.ScreenResolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw/screenh;
            var width = height*ratio;


            float w = ((float)Size.Width / width);
            float h = ((float)Size.Height / height);
            float x = ((float)Position.X / width) + w * 0.5f;
            float y = ((float)Position.Y / height) + h * 0.5f;
            
            Function.Call(Hash.DRAW_SPRITE, TextureDict, TextureName, x, y, w, h, Heading, Color.R, Color.G, Color.B, Color.A);
        }

        public static void DrawTexture(string path, Point position, Size size)
        {
            int screenw = Game.ScreenResolution.Width;
            int screenh = Game.ScreenResolution.Height;
            float height = 1080f;
            float ratio = (float)screenw / screenh;
            float width = height * ratio;
            
            float reduceX = UI.WIDTH/(float)width;
            float reduceY = UI.HEIGHT / (float)height;

            int sizeH = Convert.ToInt32((size.Height / ratio)*reduceY);

            UI.DrawTexture(path, 1, 1, 50, new Point(Convert.ToInt32(position.X*reduceX), Convert.ToInt32(position.Y*reduceY)), new Size(Convert.ToInt32(size.Width * reduceX), sizeH));
        }

        public static string WriteFileFromResources(Assembly yourAssembly, string fullResourceName)
        {
            string tmpPath = Path.GetTempFileName();
            return WriteFileFromResources(yourAssembly, fullResourceName, tmpPath);
        }

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