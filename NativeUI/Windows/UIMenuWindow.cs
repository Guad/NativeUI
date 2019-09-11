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
		internal Sprite Background;

		public virtual bool Enabled { get; set; }
		internal virtual void Position(float y)
		{
		}
		internal virtual void UpdateParent()
		{
		}
		internal virtual void Draw()
		{
		}

		public void SetParentMenu(UIMenu Menu)
		{
			ParentMenu = Menu;
		}

	}
}
