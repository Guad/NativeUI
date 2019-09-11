using System.Collections.Generic;
using System.Drawing;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using CitizenFX.Core;
using System.Threading.Tasks;

namespace NativeUI
{
	public class UIMenuColorPanel : UIMenuPanel
	{
		public ColorPanelData Data = new ColorPanelData();
		public List<UIResRectangle> Bar = new List<UIResRectangle>();
		public bool EnableArrow;
		public Sprite LeftArrow;
		public Sprite RightArrow;
		public UIResRectangle SelectedRectangle;
		public UIResText Text;
		private List<Color> Colors = new List<Color>();
		int r = 0;
		int g = 0;
		int b = 0;
		public enum ColorPanelType { Hair, Makeup }
		public ColorPanelType ColorPanelColorType;
		public int CurrentSelection
		{
			get
			{
				if (Data.Items.Count == 0)
					return 0;
				else if (Data.Index % Data.Items.Count == 0)
					return 0;
				else
					return Data.Index % Data.Items.Count;
			}
			set
			{
				if (Data.Items.Count == 0)
					Data.Index = 0;
				Data.Index = 1000000 - (1000000 % Data.Items.Count) + value;
				if (CurrentSelection > Data.Pagination.Max)
				{
					Data.Pagination.Min = CurrentSelection - (Data.Pagination.Total + 1);
					Data.Pagination.Max = CurrentSelection;
				}
				else if (CurrentSelection < Data.Pagination.Min)
				{
					Data.Pagination.Min = CurrentSelection - 1;
					Data.Pagination.Max = CurrentSelection + Data.Pagination.Total + 1;
				}
				UpdateSelection(false);
			}
		}

		public UIMenuColorPanel(string Title, ColorPanelType ColorType)
		{
			if (ColorType == ColorPanelType.Hair)
			{
				Colors.Clear();
				for (int i = 0; i < 64; i++)
				{
					API.GetHairRgbColor(i, ref r, ref g, ref b);
					Colors.Add(Color.FromArgb(r, g, b));
				}

			}
			else if (ColorType == ColorPanelType.Makeup)
			{
				Colors.Clear();
				for (int i = 0; i < 64; i++)
				{
					API.GetMakeupRgbColor(i, ref r, ref g, ref b);
					Colors.Add(Color.FromArgb(r, g, b));
				}
			}
			Data.Pagination.Min = 0;
			Data.Pagination.Max = 7;
			Data.Pagination.Total = 7;
			Data.Index = 1000;
			Data.Items = Colors;
			Data.Title = Title ?? "Title";
			Enabled = true;
			Data.Value = 1;
			Background = new Sprite("commonmenu", "gradient_bgd", new Point(0, 0), new Size(431, 122));
			EnableArrow = true;
			LeftArrow = new Sprite("commonmenu", "arrowleft", new Point(0, 0), new Size(30, 30));
			RightArrow = new Sprite("commonmenu", "arrowright", new Point(0, 0), new Size(30, 30));
			SelectedRectangle = new UIResRectangle(new Point(0, 0), new Size(44, 8), Color.FromArgb(255, 255, 255));
			Text = new UIResText(Title + " [1 / " + Colors.Count + "]", new Point(0, 0), 0.35f, Color.FromArgb(255, 255, 255, 255), CitizenFX.Core.UI.Font.ChaletLondon, Alignment.Center);
			ParentItem = null;

			for (int Index = 0; Index < Colors.Count; Index++)
			{
				if (Index < 9)
					Bar.Add(new UIResRectangle(new PointF(0, 0), new SizeF(44.5f, 44.5f), Colors[Index]));
				else
					break;
			}
			if (Data.Items.Count != 0)
			{
				Data.Index = 1000 - (1000 % Data.Items.Count);
				Data.Pagination.Max = Data.Pagination.Total + 1;
				Data.Pagination.Min = 0;
			}
		}

		public override void Position(float y)
		{
//			float Y = y - 80;
			float Y = y;
			float ParentOffsetX = ParentItem.Offset.X; float ParentOffsetWidth = ParentItem.Parent.WidthOffset;
			Background.Position = new PointF(ParentOffsetX, Y);
			for (int Index = 0; Index < Bar.Count; Index++)
				Bar[Index].Position = new PointF(15f + (44.5f * Index) + ParentOffsetX + (ParentOffsetWidth / 2), 55f + Y);
			SelectedRectangle.Position = new PointF(15f + (44.5f * (CurrentSelection - Data.Pagination.Min)) + ParentOffsetX + (ParentOffsetWidth / 2), 47f + Y);
			if (EnableArrow)
			{
				LeftArrow.Position = new PointF(7.5f + ParentOffsetX + (ParentOffsetWidth / 2), 15f + Y);
				RightArrow.Position = new PointF(393.5f + ParentOffsetX + (ParentOffsetWidth / 2), 15f + Y);
			}
			Text.Position = new PointF(215.5f + ParentOffsetX + (ParentOffsetWidth / 2), 15f + Y);
		}

		private void UpdateSelection(bool update)
		{
			if (update)
			{
				ParentItem.Parent.ListChange(ParentItem, ParentItem.Index);
				ParentItem.ListChangedTrigger(ParentItem.Index);
			}
			SelectedRectangle.Position = new PointF(15f + (44.5f * ((CurrentSelection - Data.Pagination.Min))) + ParentItem.Offset.X, SelectedRectangle.Position.Y);
			for (int index = 0; index < 9; index++)
				Bar[index].Color = Data.Items[Data.Pagination.Min + index];
			Text.Caption = Data.Title + " [" + (CurrentSelection + 1) + " / " + (Data.Items.Count) + "]";
		}

		private void Functions()
		{
			if (Screen.IsMouseInBounds(LeftArrow.Position, LeftArrow.Size))
				if (API.IsDisabledControlJustPressed(0, 24) || API.IsControlJustPressed(0, 24))
					GoLeft();
			if (Screen.IsMouseInBounds(RightArrow.Position, RightArrow.Size))
				if (API.IsDisabledControlJustPressed(0, 24) || API.IsControlJustPressed(0, 24))
					GoRight();
			for (int Index = 0; Index < Bar.Count; Index++)
			{
				if (Screen.IsMouseInBounds(Bar[Index].Position, Bar[Index].Size))
					if (API.IsDisabledControlJustPressed(0, 24) || API.IsControlJustPressed(0, 24))
					{
						CurrentSelection = Data.Pagination.Min + Index;
						UpdateSelection(true);
					}
			}
		}

		private void GoLeft()
		{
			if (Data.Items.Count > Data.Pagination.Total + 1)
			{
				if (CurrentSelection <= Data.Pagination.Min)
				{
					if (CurrentSelection == 0)
					{
						Data.Pagination.Min = Data.Items.Count - (Data.Pagination.Total + 1) - 1;
						Data.Pagination.Max = Data.Items.Count - 1;
						Data.Index = 1000 - (1000 % Data.Items.Count);
						Data.Index += (Data.Items.Count - 1);
						UpdateSelection(true);
					}
					else
					{
						Data.Pagination.Min -= 1;
						Data.Pagination.Max -= 1;
						Data.Index -= 1;
						UpdateSelection(true);
					}
				}
				else
				{
					Data.Index -= 1;
					UpdateSelection(true);
				}
			}
			else
			{
				Data.Index -= 1;
				UpdateSelection(true);
			}
		}

		private void GoRight()
		{
			if (Data.Items.Count > Data.Pagination.Total + 1)
			{
				if (CurrentSelection >= Data.Pagination.Max)
				{
					if (CurrentSelection == Data.Items.Count -1)
					{
						Data.Pagination.Min = 0;
						Data.Pagination.Max = Data.Pagination.Total + 1;
						Data.Index = 1000 - (1000 % Data.Items.Count);
						UpdateSelection(true);
					}
					else
					{
						Data.Pagination.Max += 1;
						Data.Pagination.Min = Data.Pagination.Max - (Data.Pagination.Total + 1);
						Data.Index += 1;
						UpdateSelection(true);
					}
				}
				else
				{
					Data.Index += 1;
					UpdateSelection(true);
				}
			}
			else
			{
				Data.Index += 1;
				UpdateSelection(true);
			}
		}

		public async override Task Draw()
		{
			if (Enabled)
			{
				Background.Size = new Size(431 + ParentItem.Parent.WidthOffset, 112);
				Background.Draw();
				if (EnableArrow)
				{
					LeftArrow.Draw();
					RightArrow.Draw();
				}
				Text.Draw();
				for (int Index = 0; Index < Bar.Count; Index++)
					Bar[Index].Draw();
				SelectedRectangle.Draw();
				Functions();
			}
			await Task.FromResult(0);
		}
	}



	public class ColorPanelData
	{
		public Pagination Pagination = new Pagination();
		public int Index;
		public List<Color> Items;
		public string Title;
		public bool Enabled;
		public int Value;
	}

	public class Pagination
	{
		public int Min;
		public int Max;
		public int Total;
	}
}
