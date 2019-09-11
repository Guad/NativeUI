using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace NativeUI
{
	public class UIMenuProgressItem : UIMenuItem
	{
		private UIResRectangle _background;
		private List<dynamic> Items = new List<dynamic>();
		public int Index
		{
			get { return Index; }
			set
			{
				if (value > Items.Count)
					Index = 0;
				else if (value < 0)
					Index = Items.Count;
				else
					Index = value;
				if (Counter)
					RightLabel = Index + "/" + Items.Count;
				else
					RightLabel = ""+Items[Index];
			}
		}
		public bool Pressed;
		private bool Counter;
		public float Max = 407.5f;
		public UIMenuGridAudio Audio;
		public UIResRectangle Bar;


		public event OnProgressChanged OnProgressChanged;
		public event OnProgressSelected OnProgressSelected;

		public UIMenuProgressItem(string text, string description, List<dynamic> items, int index, bool counter) : base(text, description)
		{
			Items = items;
			Index = index;
			Counter = counter;
			Max = 407.5f;
			Audio = new UIMenuGridAudio("CONTINUOUS_SLIDER", "HUD_FRONTEND_DEFAULT_SOUNDSET", 0);
			_background = new UIResRectangle(new PointF(0, 0), new Size(415, 20), Color.FromArgb(0, 0, 0, 255));
			Bar = new UIResRectangle(new PointF(0, 0), new SizeF(407.5f, 12.5f));
			_rectangle.Size = new SizeF(_rectangle.Size.Width, 60);
			_selectedSprite.Size = new SizeF(_selectedSprite.Size.Width, 60);
			if (Counter)
				RightLabel = Index + "/" + Items.Count;
			else
				RightLabel = ""+Items[Index];

			Bar.Size = new SizeF(Index / Items.Count * Max, Bar.Size.Height);
		}

		public void ProgressChanged(UIMenu Menu, UIMenuProgressItem Item, int index)
		{
			OnProgressChanged?.Invoke(Menu, Item, index);
		}

		public void ProgressSelected(UIMenu Menu, UIMenuProgressItem Item, int index)
		{
			OnProgressSelected?.Invoke(Menu, Item, index);
		}

		public override void Position(int y)
		{
			base.Position(y);
			Max = 407.5f + Parent.WidthOffset;
			_background.Size = new SizeF(415f + Parent.WidthOffset, 20f);
			_background.Position = new PointF(8f + Offset.X, 177f + y + Offset.Y);
			Bar.Position = new PointF(11.75f + Offset.X, 180.75f + y + Offset.Y);
		}


		[Obsolete("Use UIMenuProgressItem.Items.FindIndex(p => ReferenceEquals(p, item)) instead.")]
		public dynamic ItemToIndex(dynamic item)
		{
			return Items.FindIndex(p => ReferenceEquals(p, item));
		}

		[Obsolete("Use UIMenuProgressItem.Items[Index] instead.")]
		public dynamic IndexToItem(int index)
		{
			return Items[index];
		}

		public override void SetRightBadge(BadgeStyle badge)
		{
			throw new Exception("UIMenuProgressItem cannot have a right badge.");
		}

		public override void SetRightLabel(string text)
		{
			throw new Exception("UIMenuProgressItem cannot have a right label.");
		}

		public void CalculateProgress(float CursorX)
		{
			var Progress = CursorX - Bar.Position.X;
			Index = (int)Math.Round(Items.Count * ((Progress >= 0f && Progress <= Max ) ? Progress : (Progress < 0) ? 0 : Max) / Max);
		}

		public async void Functions()
		{
			if (Screen.IsMouseInBounds(new PointF((int)Math.Round(Bar.Position.X), (int)Math.Round(Bar.Position.Y - 12)), new Size((int)Max, (int)Math.Round(Bar.Size.Height + 24)), Offset))
			{
				if (API.IsDisabledControlPressed(0, 24))
				{
					var ress = Screen.ResolutionMaintainRatio;
					float CursorX = (float)Math.Round(API.GetControlNormal(0, 239) * ress.Width);
					CalculateProgress(CursorX);
					Parent.ProgressChange(this, Index);
					ProgressChanged(Parent, this, Index);
					if (!Pressed)
					{
						Pressed = true;
						Audio.Id = API.GetSoundId();
						API.PlaySoundFrontend(Audio.Id, Audio.Slider, Audio.Library, true);
						await BaseScript.Delay(100);
						API.StopSound(Audio.Id);
						API.ReleaseSoundId(Audio.Id);
						Pressed = false;
					}
				}
			}
		}

		public async override Task Draw()
		{
			await base.Draw();
			if (Selected)
			{
				_background.Color = Colors.Black;
				Bar.Color = Colors.White;
			}
			else
			{
				_background.Color = Colors.White;
				Bar.Color = Colors.Black;
			}
			_background.Draw();
			Bar.Draw();
			Functions();
			await Task.FromResult(0);
		}
	}
}
