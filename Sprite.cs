using System;
using System.Drawing;
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

        public string TextureDict;
        public string TextureName;

        public Sprite(string textureDict, string textureName, Point position, Size size, float heading, Color color) //BASE
        {
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


    }
}