using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace NativeUI
{
	public class UIMenuVerticalOneLineGridPanel : UIMenuPanel
	{
		private UIResText Top;
		private UIResText Bottom;
		private Sprite Grid;
		private Sprite Circle;
		private UIMenuGridAudio Audio;
		private PointF SetCirclePosition;
		protected bool CircleLocked;
		protected bool Pressed;
		public PointF CirclePosition
		{
			get
			{
				return new PointF((float)Math.Round((Circle.Position.X - (Grid.Position.X + 20) + (Circle.Size.Width / 2)) / (Grid.Size.Width - 40), 2), (float)Math.Round((Circle.Position.Y - (Grid.Position.Y + 20) + (Circle.Size.Height / 2)) / (Grid.Size.Height - 40), 2));
			}
			set
			{
				Circle.Position.X = (Grid.Position.X + 20) + ((Grid.Size.Width - 40) * (value.X >= 0f && value.X <= 1.0f ? value.X : 0.0f)) - (Circle.Size.Width / 2);
				Circle.Position.Y = (Grid.Position.Y + 20) + ((Grid.Size.Height - 40) * (value.Y >= 0f && value.Y <= 1.0f ? value.Y : 0.0f)) - (Circle.Size.Height / 2);
			}
		}

		public UIMenuVerticalOneLineGridPanel(string TopText, string BottomText, float circlePositionY = .5f)
		{
			Enabled = true;
			Background = new Sprite("commonmenu", "gradient_bgd", new Point(0, 0), new Size(431, 275));
			Grid = new Sprite("NativeUI", "vertical_grid", new Point(0, 0), new Size(200, 200), 0f, Color.FromArgb(255, 255, 255));
			Circle = new Sprite("mpinventory", "in_world_circle", new Point(0, 0), new Size(20, 20), 0f, Color.FromArgb(255, 255, 255));
			Audio = new UIMenuGridAudio("CONTINUOUS_SLIDER", "HUD_FRONTEND_DEFAULT_SOUNDSET", 0);
			Top = new UIResText(TopText ?? "Sopra", new Point(0, 0), .35f, Color.FromArgb(255, 255, 255), CitizenFX.Core.UI.Font.ChaletLondon, Alignment.Center);
			Bottom = new UIResText(BottomText ?? "Sotto", new Point(0, 0), .35f, Color.FromArgb(255, 255, 255), CitizenFX.Core.UI.Font.ChaletLondon, Alignment.Center);
			SetCirclePosition = new PointF(.5f, circlePositionY != 0 ? circlePositionY : .5f);
		}

		public override void Position(float y)
		{
			//var Y = y - 205;
			float Y = y;
			var ParentOffsetX = ParentItem.Offset.X;
			var ParentOffsetWidth = ParentItem.Parent.WidthOffset;
			Background.Position = new PointF(ParentOffsetX, Y);
			Grid.Position = new PointF(ParentOffsetX + 115.5f + (ParentOffsetWidth / 2), 37.5f + Y);
			Top.Position = new PointF(ParentOffsetX + 215.5f + (ParentOffsetWidth / 2), 5f + Y);
			Bottom.Position = new PointF(ParentOffsetX + 215.5f + (ParentOffsetWidth / 2), 240f + Y);
			if (!CircleLocked)
			{
				CircleLocked = true;
				CirclePosition = SetCirclePosition;
			}
		}

		private void UpdateParent(float Y)
		{
			ParentItem.Parent.ListChange(ParentItem, ParentItem.Index);
			ParentItem.ListChangedTrigger(ParentItem.Index);
		}

		private async void Functions()
		{
			if (Screen.IsMouseInBounds(new PointF(Grid.Position.X + 20f, Grid.Position.Y + 20f), new SizeF(Grid.Size.Width - 40f, Grid.Size.Height - 40f)))
			{
				if (API.IsDisabledControlPressed(0, 24))
				{
					if (!Pressed)
					{
						Pressed = true;
						Audio.Id = API.GetSoundId();
						API.PlaySoundFrontend(Audio.Id, Audio.Slider, Audio.Library, true);
						while (API.IsDisabledControlPressed(0, 24) && Screen.IsMouseInBounds(new PointF(Grid.Position.X + 20f, Grid.Position.Y + 20f), new SizeF(Grid.Size.Width - 40f, Grid.Size.Height - 40f)))
						{
							await BaseScript.Delay(0);
							var res = Screen.ResolutionMaintainRatio;
							float mouseX = API.GetControlNormal(0, 239) * res.Width;
							float mouseY = API.GetControlNormal(0, 240) * res.Height;

							mouseX -= (Circle.Size.Width / 2); mouseY -= (Circle.Size.Height / 2);
							Circle.Position = new PointF(mouseX > (Grid.Position.X + 10 + Grid.Size.Width - 40) ? (Grid.Position.X + 10 + Grid.Size.Width - 40) : ((mouseX < (Grid.Position.X + 20 - (Circle.Size.Width / 2))) ? (Grid.Position.X + 20 - (Circle.Size.Width / 2)) : mouseX), (mouseY > (Grid.Position.Y + 10 + Grid.Size.Height - 40) ? (Grid.Position.Y + 10 + Grid.Size.Height - 40) : ((mouseY < (Grid.Position.Y + 20 - (Circle.Size.Height / 2))) ? (Grid.Position.Y + 20 - (Circle.Size.Height / 2)) : mouseY)));
							var resultY = (float)Math.Round((Circle.Position.Y - (Grid.Position.Y + 20) + (Circle.Size.Height + 20)) / (Grid.Size.Height - 40), 2);
							UpdateParent(((resultY >= 0.0f && resultY <= 1.0f) ? resultY : ((resultY <= 0f) ? 0.0f : 1.0f) * 2f) - 1f);
						}
						API.StopSound(Audio.Id);
						API.ReleaseSoundId(Audio.Id);
						Pressed = false;
					}
				}
			}
		}

		public async override Task Draw()
		{
			if (!Enabled) return;
			Background.Size = new Size(431 + ParentItem.Parent.WidthOffset, 275);
			Background.Draw();
			Grid.Draw();
			Circle.Draw();
			Top.Draw();
			Bottom.Draw();
			Functions();
			await Task.FromResult(0);
		}
	}
}
