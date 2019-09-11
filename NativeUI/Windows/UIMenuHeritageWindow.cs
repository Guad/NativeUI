using System;
using System.Collections.Generic;
using System.Drawing;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace NativeUI
{
	public class UIMenuHeritageWindow : UIMenuWindow
	{
		private Sprite MomSprite;
		private Sprite DadSprite;
		private int Mom;
		private int Dad;
		public PointF Offset;

		public UIMenuHeritageWindow(int mom, int dad)
		{
			Background = new Sprite("pause_menu_pages_char_mom_dad", "mumdadbg", new Point(0, 0), new Size(431, 228));
			Mom = mom;
			Dad = dad;
			MomSprite = new Sprite("char_creator_portraits", (Mom < 21)? "female_" + (Mom) : "special_female_" + (Mom - 21), new Point(0, 0), new Size(228, 228));
			DadSprite = new Sprite("char_creator_portraits", (Dad < 21)? "Male_" + (Dad) : "special_male_" + (Dad-21), new Point(0, 0), new Size(228, 228));
			Offset = new PointF(0, 0);
		}

		internal override void Position(float y)
		{
			Background.Position = new PointF(Offset.X, 70 + y + ParentMenu.Offset.Y);
			MomSprite.Position = new PointF(Offset.X + (ParentMenu.WidthOffset / 2) + 25, 70 + y + ParentMenu.Offset.Y);
			DadSprite.Position = new PointF(Offset.X + (ParentMenu.WidthOffset / 2) + 195, 70 + y + ParentMenu.Offset.Y);
		}

		public void Index(int mom, int dad)
		{
			Mom = mom;
			Dad = dad;
			MomSprite.TextureName = (Mom < 21) ? "female_" + Mom : "special_female_" + (Mom - 21);
			DadSprite.TextureName = (Dad < 21) ? "male_" + Dad : "special_male_" + (Dad - 21);
		}

		internal override void Draw()
		{
			Background.Size = new Size(431 + ParentMenu.WidthOffset, 228);
			Background.Draw();
			DadSprite.Draw();
			MomSprite.Draw();
		}
	}
}
