using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core.UI;

namespace NativeUI
{
	public class UIMenuStatisticsPanel : UIMenuPanel
	{
		private List<StatisticsForPanel> Items = new List<StatisticsForPanel>();
		private bool Divider;

		public UIMenuStatisticsPanel()
		{
			Background = new UIResRectangle(new Point(0, 0), new Size(431, 47), Color.FromArgb(170, 0, 0, 0));
			Divider = true;
		}

		public void AddStatistics (string Name)
		{
			StatisticsForPanel items = new StatisticsForPanel
			{
				Text = new UIResText(Name ?? "", new Point(0, 0), .35f, Color.FromArgb(255, 255, 255), CitizenFX.Core.UI.Font.ChaletLondon, Alignment.Left),
				BackgroundProgressBar = new UIResRectangle(new Point(0, 0), new Size(200, 10), Color.FromArgb(100, 255, 255, 255)),
				ProgressBar = new UIResRectangle(new Point(0, 0), new Size(100, 10), Color.FromArgb(255, 255, 255, 255)),
				Divider = new UIResRectangle[5]
				{
					new UIResRectangle(new Point(0,0), new Size(2,10), Color.FromArgb(255, 0,0,0)),
					new UIResRectangle(new Point(0,0), new Size(2,10), Color.FromArgb(255, 0,0,0)),
					new UIResRectangle(new Point(0,0), new Size(2,10), Color.FromArgb(255, 0,0,0)),
					new UIResRectangle(new Point(0,0), new Size(2,10), Color.FromArgb(255, 0,0,0)),
					new UIResRectangle(new Point(0,0), new Size(2,10), Color.FromArgb(255, 0,0,0))
				}
			};
			Items.Add(items);
		}

		public float GetPercentage(int ItemId)
		{
			return Items[ItemId].ProgressBar.Size.Width * 2f;
		}

		public void SetPercentage(int ItemId, float number)
		{
			if (number <= 0)
				Items[ItemId].ProgressBar.Size = new SizeF(0, Items[ItemId].ProgressBar.Size.Height);
			else
			{
				if (number <= 100)
					Items[ItemId].ProgressBar.Size = new SizeF(number * 2f, Items[ItemId].ProgressBar.Size.Height);
				else
					Items[ItemId].ProgressBar.Size = new SizeF(100f * 2f, Items[ItemId].ProgressBar.Size.Height);

			}
		}

		internal override void Position(float y)
		{
			float Y = y;
			var ParentOffsetX = ParentItem.Offset.X;
			var ParentOffsetWidth = ParentItem.Parent.WidthOffset;
			Background.Position = new PointF(ParentOffsetX, Y);
			for (int i=0; i<Items.Count; i++)
			{
				var OffsetItemCount = 40 * (i + 1);
				Items[i].Text.Position = new PointF(ParentOffsetX + (ParentOffsetWidth / 2) + 13, Y - 34 + OffsetItemCount);
				Items[i].BackgroundProgressBar.Position = new PointF(ParentOffsetX + (ParentOffsetWidth / 2) + 200, Y - 22 + OffsetItemCount);
				Items[i].ProgressBar.Position = new PointF(ParentOffsetX + (ParentOffsetWidth / 2) + 200, Y - 22 + OffsetItemCount);
				if (Divider)
				{
					for (int _=0; _<Items[i].Divider.Length; _++)
					{
						var DividerOffsetWidth = _ * 40;
						Items[i].Divider[_].Position = new PointF(ParentOffsetX + (ParentOffsetWidth / 2) + 200 + DividerOffsetWidth, Y - 22 + OffsetItemCount);
						Background.Size = new SizeF(431 + ParentItem.Parent.WidthOffset, 47 + OffsetItemCount - 39);
					}
				}
			}
		}

		internal async override Task Draw()
		{
			Background.Draw();
			for (int i=0; i<Items.Count; i++)
			{
				Items[i].Text.Draw();
				Items[i].BackgroundProgressBar.Draw();
				Items[i].ProgressBar.Draw();
				for (int _ = 0; _ < Items[i].Divider.Length; _++)
					Items[i].Divider[_].Draw();
			}
			await Task.FromResult(0);
		}
	}

	public class StatisticsForPanel
	{
		public UIResText Text;
		public UIResRectangle BackgroundProgressBar;
		public UIResRectangle ProgressBar;
		public UIResRectangle[] Divider = new UIResRectangle[5];
	}
}
