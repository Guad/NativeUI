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
            /*
                   const float w = static_cast<float>(this->Size.Width) / UI::WIDTH;
           const float h = static_cast<float>(this->Size.Height) / UI::HEIGHT;
           const float x = ((static_cast<float>(this->Position.X) + offset.Width) / UI::WIDTH) + w * 0.5f;
           const float y = ((static_cast<float>(this->Position.Y) + offset.Height) / UI::HEIGHT) + h * 0.5f;

           Native::Function::Call(Native::Hash::DRAW_RECT, x, y, w, h, this->Color.R, this->Color.G, this->Color.B, this->Color.A);
                   */
            int width = Game.ScreenResolution.Width;
            int height = Game.ScreenResolution.Height;
            /*
                float w = ((float)Size.Width / UI.WIDTH);
                float h = ((float)Size.Height / UI.HEIGHT);
                float x = ((float)Position.X / UI.WIDTH) + w * 0.5f;
                float y = ((float)Position.Y / UI.HEIGHT) + h * 0.5f;
                */
            float w = ((float)Size.Width / width);
            float h = ((float)Size.Height / height);
            float x = ((float)Position.X / width) + w * 0.5f;
            float y = ((float)Position.Y / height) + h * 0.5f;
            Function.Call(Hash.DRAW_SPRITE, TextureDict, TextureName, x, y, w, h, Heading, Color.R, Color.G, Color.B, Color.A);
        }


    }
}