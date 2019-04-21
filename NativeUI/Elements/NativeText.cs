using GTA.Native;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace NativeUI.Elements
{
    /// <summary>
    /// The alignment of the text to draw.
    /// </summary>
    public enum TextAlignment
    {
        Left,
        Centered,
        Right,
    }

    /// <summary>
    /// New Design for the NativeUI texts.
    /// </summary>
    public class NativeText : NativeElement
    {
        #region Consistent Values

        /// <summary>
        /// Max size of every Caption chunk.
        /// </summary>
        private const int ChunkSize = 90;

        #endregion

        #region Private Properties

        /// <summary>
        /// The raw word wrap value.
        /// </summary>
        private float InternalWordWrap = 0f;
        /// <summary>
        /// The already calculated word wrapping.
        /// </summary>
        private float CalculatedWrap = 0f;
        /// <summary>
        /// Internal width calculation.
        /// </summary>
        private float Width = 0f;
        /// <summary>
        /// List of chunks of the Caption.
        /// </summary>
        private List<string> Chunks = new List<string>();
        /// <summary>
        /// Internal storage of the caption.
        /// </summary>
        private string _Caption = "";

        #endregion

        #region Public Properties

        /// <summary>
        /// The UTF-8 string to draw on the screen.
        /// </summary>
        public string Caption
        {
            get => _Caption;
            set // F
            {
                _Caption = value;
                Chunks.Clear(); // Wipe the list
                double TotalParts = Math.Ceiling(value.Length / (float)ChunkSize); // Get the number of parts that we need
                if (TotalParts == 1) // If we only need one part
                {
                    Chunks.Add(value); // Add the complete string
                }
                else // Otherwise
                {
                    for (int i = 0; i < TotalParts; i++) // Iterate over the number of parts that we need
                    {
                        int Start = i * ChunkSize; // Calculate the start
                        // If the Start + Aditional Chunk is higher than the length of the string, add the characters left
                        int End = (Start + ChunkSize) > value.Length ? value.Length - Start : ChunkSize;
                        Chunks.Add(value.Substring(Start, End)); // Finally add the string
                    }
                }
            }
        }
        /// <summary>
        /// The Internal GTA Font to be used.
        /// </summary>
        public GTA.Font Font { get; set; } = GTA.Font.ChaletLondon;
        /// <summary>
        /// The scale of the text between 0.5f and 3.0f.
        /// </summary>
        public float Scale { get; set; } = 1f;
        /// <summary>
        /// If the text should be drawn with a shadow.
        /// </summary>
        public bool Shadow { get; set; } = false;
        /// <summary>
        /// If the text should have an outline.
        /// </summary>
        public bool Outline { get; set; } = false;
        /// <summary>
        /// The alignment of the text.
        /// </summary>
        public TextAlignment Alignment { get; set; } = TextAlignment.Left;
        /// <summary>
        /// The maximum width for word wrapping.
        /// Set to zero to disable.
        /// </summary>
        public float WordWrap
        {
            get
            {
                return InternalWordWrap;
            }
            set
            {
                InternalWordWrap = value;
                CalculatedWrap = (Position.X + value) / Width;
            }
        }

        #endregion

        #region Constructors

        public NativeText(string caption, Point position) : this(caption, position, 1, Color.White, GTA.Font.ChaletLondon, TextAlignment.Left) { }

        public NativeText(string caption, Point position, float scale) : this(caption, position, scale, Color.White, GTA.Font.ChaletLondon, TextAlignment.Left) { }

        public NativeText(string caption, Point position, float scale, Color color) : this(caption, position, scale, color, GTA.Font.ChaletLondon, TextAlignment.Left) { }

        public NativeText(string caption, Point position, float scale, Color color, GTA.Font font) : this(caption, position, scale, color, font, TextAlignment.Left) { }

        public NativeText(string caption, Point position, float scale, Color color, GTA.Font font, TextAlignment alignment)
        {
            Caption = caption;
            Scale = scale;
            Color = color;
            Font = font;
            Alignment = alignment;
            Position = position;
        }

        #endregion

        #region Drawing
                
        /// <summary>
        /// Draws the text on screen with the specified offset.
        /// </summary>
        public override void Draw()
        {
            // Set some basics
            Function.Call(Hash.SET_TEXT_FONT, (int)Font);
            Function.Call(Hash.SET_TEXT_SCALE, 1.0f, Scale);
            Function.Call(Hash.SET_TEXT_COLOUR, Color.R, Color.G, Color.B, Color.A);
            if (Shadow) Function.Call(Hash.SET_TEXT_DROP_SHADOW);
            if (Outline) Function.Call(Hash.SET_TEXT_OUTLINE);

            // Change the alignment to the correct value
            switch (Alignment)
            {
                case TextAlignment.Centered:
                    Function.Call(Hash.SET_TEXT_CENTRE, true);
                    break;
                case TextAlignment.Right:
                    Function.Call(Hash.SET_TEXT_RIGHT_JUSTIFY, true);
                    Function.Call(Hash.SET_TEXT_WRAP, 0, RelativePosition.X);
                    break;
            }

            // Set the correct word wrap acording to our calculations
            if (WordWrap != 0)
            {
                Function.Call(Hash.SET_TEXT_WRAP, RelativePosition.X, CalculatedWrap);
            }

            // Time to draw everything!
            // Call BEGIN_TEXT_COMMAND_DISPLAY_TEXT to start sending text
            Function.Call(Hash._SET_TEXT_ENTRY, "CELL_EMAIL_BCON");
            // Then, send the string on chunks of 90 characters
            foreach (string Chunk in Chunks)
            {
                Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, Chunk);
            }
            // And draw it
            Function.Call(Hash._DRAW_TEXT, RelativePosition.X, RelativePosition.Y);
        }

        #endregion
    }
}
