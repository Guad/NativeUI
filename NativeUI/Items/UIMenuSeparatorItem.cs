using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeUI
{
	public class UIMenuSeparatorItem : UIMenuItem
	{
		/// <summary>
		/// Use it to create an Empty item to separate menu Items
		/// </summary>
		public UIMenuSeparatorItem() : base("", "")
		{
		}
	}
}
