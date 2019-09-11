using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeUI
{
	public class UIMenuWindow
	{
		public UIMenu ParentMenu { get; set; }
		public Sprite Background;

		public virtual bool Enabled { get; set; }
		public virtual void Position(float y)
		{
		}
		public virtual void UpdateParent()
		{
		}
		public virtual void Draw()
		{
		}

		public void SetParentMenu(UIMenu Menu)
		{
			ParentMenu = Menu;
		}

	}
}
