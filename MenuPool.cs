using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace NativeUI
{
    public static class MenuPool
    {
        public static bool ControllerUsed { get; set; }

        private static readonly List<UIMenu> MenuList = new List<UIMenu>();

        public static void Add(UIMenu menu)
        {
            MenuList.Add(menu);
        }

        public static List<UIMenu> ToList()
        {
            return MenuList;
        }

        public static void ProcessControl()
        {
            foreach (var menu in MenuList.Where(menu => menu.Visible))
            {
                menu.ProcessControl();
            }
        }

        public static void ProcessKey(Keys key)
        {
            foreach (var menu in MenuList.Where(menu => menu.Visible))
            {
                menu.ProcessKey(key);
            }
        }

        public static void ProcessMouse()
        {
            foreach (var menu in MenuList.Where(menu => menu.Visible))
            {
                menu.ProcessMouse();
            }
        }

        public static void Draw()
        {
            foreach (var menu in MenuList.Where(menu => menu.Visible))
            {
                menu.Draw();
            }
        }

        public static void ProcessMenus()
        {
            ProcessControl();
            ProcessMouse();
            Draw();
        }
    }
}