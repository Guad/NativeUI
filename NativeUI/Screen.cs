using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using System.Drawing;
using System;

namespace NativeUI
{
	/// <summary>
	/// Tools to deal with the game screen.
	/// </summary>
	public static class Screen
	{
		/// <summary>
		/// The 1080pixels-based screen resolution while mantaining current aspect ratio.
		/// </summary>
		public static SizeF ResolutionMaintainRatio
		{
			get
			{
				// Get the game width and height
				int screenw = CitizenFX.Core.UI.Screen.Resolution.Width;
				int screenh = CitizenFX.Core.UI.Screen.Resolution.Height;
				// Calculate the ratio
				float ratio = (float)screenw / screenh;
				// And the width with that ratio
				float width = 1080f * ratio;
				// Finally, return a SizeF
				return new SizeF(width, 1080f);
			}
		}

		/// <summary>
		/// Chech whether the mouse is inside the specified rectangle.
		/// </summary>
		/// <param name="topLeft">Start point of the rectangle at the top left.</param>
		/// <param name="boxSize">size of your rectangle.</param>
		/// <returns>true if the mouse is inside of the specified bounds, false otherwise.</returns>
		public static bool IsMouseInBounds(Point topLeft, Size boxSize)
		{
			Game.EnableControlThisFrame(0, Control.CursorX);
			Game.EnableControlThisFrame(0, Control.CursorY);
			// Get the resolution while maintaining the ratio.
			SizeF res = ResolutionMaintainRatio;
			// Then, get the position of mouse on the screen while relative to the current resolution
			int mouseX = (int)Math.Round(API.GetControlNormal(0, 239) * res.Width);
			int mouseY = (int)Math.Round(API.GetControlNormal(0, 240) * res.Height);
			// And check if the mouse is on the rectangle bounds
			bool isX = mouseX >= topLeft.X && mouseX <= topLeft.X + boxSize.Width;
			bool isY = mouseY > topLeft.Y && mouseY < topLeft.Y + boxSize.Height;
			// Finally, return the result of the checks
			return isX && isY;
		}

		public static bool IsMouseInBounds(PointF topLeft, SizeF boxSize)
		{
			Game.EnableControlThisFrame(0, Control.CursorX);
			Game.EnableControlThisFrame(0, Control.CursorY);
			// Get the resolution while maintaining the ratio.
			SizeF res = ResolutionMaintainRatio;
			// Then, get the position of mouse on the screen while relative to the current resolution
			float mouseX = GetControlNormal(0, 239) * res.Width;
			float mouseY = GetControlNormal(0, 240) * res.Height;
			// And check if the mouse is on the rectangle bounds
			bool isX = mouseX >= topLeft.X && mouseX <= topLeft.X + boxSize.Width;
			bool isY = mouseY > topLeft.Y && mouseY < topLeft.Y + boxSize.Height;
			// Finally, return the result of the checks
			return isX && isY;
		}

		public static bool IsMouseInBounds(Point topLeft, Size boxSize, Point DrawOffset)
		{
			Game.EnableControlThisFrame(0, Control.CursorX);
			Game.EnableControlThisFrame(0, Control.CursorY);
			SizeF res = ResolutionMaintainRatio;

			int mouseX = (int)Math.Round(API.GetControlNormal(0, 239) * res.Width);
			int mouseY = (int)Math.Round(API.GetControlNormal(0, 240) * res.Height);

			mouseX += DrawOffset.X;
			mouseY += DrawOffset.Y;

			return (mouseX >= topLeft.X && mouseX <= topLeft.X + boxSize.Width)
				   && (mouseY > topLeft.Y && mouseY < topLeft.Y + boxSize.Height);
		}

		public static bool IsMouseInBounds(PointF topLeft, SizeF boxSize, PointF DrawOffset)
		{
			Game.EnableControlThisFrame(0, Control.CursorX);
			Game.EnableControlThisFrame(0, Control.CursorY);
			SizeF res = ResolutionMaintainRatio;

			float mouseX = GetControlNormal(0, 239) * res.Width;
			float mouseY = GetControlNormal(0, 240) * res.Height;

			mouseX += DrawOffset.X;
			mouseY += DrawOffset.Y;

			return (mouseX >= topLeft.X && mouseX <= topLeft.X + boxSize.Width)
				   && (mouseY > topLeft.Y && mouseY < topLeft.Y + boxSize.Height);
		}

		/// <summary>
		/// Returns the safezone bounds in pixel, relative to the 1080pixel based system.
		/// </summary>
		/// <returns></returns>
		/// <summary>
		/// Safezone bounds relative to the 1080pixel based resolution.
		/// </summary>
		public static Point SafezoneBounds
		{
			get
			{
				// Get the size of the safezone as a float
				float t = GetSafeZoneSize();
				// Round the value with a max of 2 decimal places and do some calculations
				double g = Math.Round(Convert.ToDouble(t), 2);
				g = (g * 100) - 90;
				g = 10 - g;

				// Then, get the screen resolution
				int screenw = CitizenFX.Core.UI.Screen.Resolution.Width;
				int screenh = CitizenFX.Core.UI.Screen.Resolution.Height;
				// Calculate the ratio
				float ratio = (float)screenw / screenh;
				// And this thing (that I don't know what it does)
				float wmp = ratio * 5.4f;

				// Finally, return a new point with the correct resolution
				return new Point((int)Math.Round(g * wmp), (int)Math.Round(g * 5.4f));
			}
		}

		/// <summary>
		/// Calculates the width of a string.
		/// </summary>
		/// <param name="text">The text to measure.</param>
		/// <param name="font">Game font used for measurements.</param>
		/// <param name="scale">The scale of the characters.</param>
		/// <returns>The width of the string based on the font and scale.</returns>
		public static float GetTextWidth(string text, CitizenFX.Core.UI.Font font, float scale)
		{
			// Start by requesting the game to start processing a width measurement
			SetTextEntryForWidth("CELL_EMAIL_BCON"); // _BEGIN_TEXT_COMMAND_WIDTH
													 // Add the text string
			UIResText.AddLongString(text);
			// Set the properties for the text
			SetTextFont((int)font);
			SetTextScale(1f, scale);

			// Ask the game for the relative string width
			float width = GetTextScreenWidth(true);
			// And return the literal result
			return ResolutionMaintainRatio.Width * width;
		}

		/// <summary>
		/// Gets the line count for the text.
		/// </summary>
		/// <param name="text">The text to measure.</param>
		/// <param name="position">The position of the text.</param>
		/// <param name="font">The font to use.</param>
		/// <returns>The number of lines used.</returns>
		public static int GetLineCount(string text, Point position, CitizenFX.Core.UI.Font font, float scale, int wrap)
		{
			// Tell the game that we are going to request the number of lines
			SetTextGxtEntry("CELL_EMAIL_BCON"); // _BEGIN_TEXT_COMMAND_LINE_COUNT
												// Add the text that has been sent to us
			UIResText.AddLongStringForUtf8(text); // ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME

			// Get the resolution with the correct aspect ratio
			SizeF res = ResolutionMaintainRatio;
			// Calculate the x and y positions
			float x = position.X / res.Width;
			float y = position.Y / res.Height;

			// Set the properties for the text
			SetTextFont((int)font);
			SetTextScale(1f, scale);

			// If there is some text wrap to add
			if (wrap > 0)
			{
				// Calculate the wrap size
				float start = position.X / res.Width;
				float end = start + (wrap / res.Width);
				// And apply it
				SetTextWrap(x, end);
			}
			// Finally, return the number of lines being made by the string
			return GetTextScreenLineCount(x, y); // _GET_TEXT_SCREEN_LINE_COUNT
		}

		public static int GetLineCount(string text, PointF position, CitizenFX.Core.UI.Font font, float scale, float wrap)
		{
			// Tell the game that we are going to request the number of lines
			SetTextGxtEntry("CELL_EMAIL_BCON"); // _BEGIN_TEXT_COMMAND_LINE_COUNT
												// Add the text that has been sent to us
			UIResText.AddLongStringForUtf8(text); // ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME

			// Get the resolution with the correct aspect ratio
			SizeF res = ResolutionMaintainRatio;
			// Calculate the x and y positions
			float x = position.X / res.Width;
			float y = position.Y / res.Height;

			// Set the properties for the text
			SetTextFont((int)font);
			SetTextScale(1f, scale);

			// If there is some text wrap to add
			if (wrap > 0)
			{
				// Calculate the wrap size
				float start = position.X / res.Width;
				float end = start + (wrap / res.Width);
				// And apply it
				SetTextWrap(x, end);
			}
			// Finally, return the number of lines being made by the string
			return GetTextScreenLineCount(x, y); // _GET_TEXT_SCREEN_LINE_COUNT
		}
	}
}

