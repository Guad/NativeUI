using System.Drawing;
using System.Threading.Tasks;

namespace NativeUI
{
	public class UIMenuSliderHeritageItem : UIMenuSliderItem
	{
		public UIMenuSliderHeritageItem(string text, string description, bool divider):base(text, description, divider)
		{
			const int y = 0;
			_arrowLeft = new Sprite("mpleaderboard", "leaderboard_female_icon", new PointF(0, 105 + y), new Size(40, 40), 0, Color.FromArgb(255,255,255));
			_arrowRight = new Sprite("mpleaderboard", "leaderboard_male_icon", new PointF(0, 105 + y), new Size(40, 40), 0, Color.FromArgb(255, 255, 255));
		}

		public override void Position(int y)
		{
			base.Position(y);
			_rectangleBackground.Position = new PointF(250f + base.Offset.X + Parent.WidthOffset, y + 158.5f + base.Offset.Y);
			_rectangleSlider.Position = new PointF(250f + base.Offset.X + Parent.WidthOffset, y + 158.5f + base.Offset.Y);
			_rectangleDivider.Position = new PointF(323.5f + base.Offset.X + Parent.WidthOffset, y + 153 + base.Offset.Y);
			_arrowLeft.Position = new PointF(217f + base.Offset.X + Parent.WidthOffset, y + 143.5f + base.Offset.Y);
			_arrowRight.Position = new PointF(395f + base.Offset.X + Parent.WidthOffset, y + 143.5f + base.Offset.Y);
		}

		public async override Task Draw()
		{
			await base.Draw();
			_arrowLeft.Color = Enabled ? Selected ? Color.FromArgb(255, 102, 178) : Colors.WhiteSmoke : Color.FromArgb(163, 159, 148);
			_arrowRight.Color = Enabled ? Selected ? Color.FromArgb(51,51,255) : Colors.WhiteSmoke : Color.FromArgb(163, 159, 148);
			if (Selected)
			{
				_arrowLeft.Draw();
				_arrowRight.Draw();
			}
			else
			{

			}
			await Task.FromResult(0);

		}
	}
}
