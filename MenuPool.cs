using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace NativeUI
{
    public class MenuPool
    {
        public static bool ControllerUsed { get; set; }

        public MenuPool()
        { }

        private readonly List<UIMenu> MenuList = new List<UIMenu>();

        public void Add(UIMenu menu)
        {
            MenuList.Add(menu);
        }

        public List<UIMenu> ToList()
        {
            return MenuList;
        }

        public void ProcessControl()
        {
            foreach (var menu in MenuList.Where(menu => menu.Visible))
            {
                menu.ProcessControl();
            }
        }

        public void ProcessKey(Keys key)
        {
            foreach (var menu in MenuList.Where(menu => menu.Visible))
            {
                menu.ProcessKey(key);
            }
        }

        public void ProcessMouse()
        {
            foreach (var menu in MenuList.Where(menu => menu.Visible))
            {
                menu.ProcessMouse();
            }
        }

        public void Draw()
        {
            foreach (var menu in MenuList.Where(menu => menu.Visible))
            {
                menu.Draw();
            }
        }

        public bool IsAnyMenuOpen()
        {
            return MenuList.Any(menu => menu.Visible);
        }

        public void ProcessMenus()
        {
            ProcessControl();
            ProcessMouse();
            Draw();
        }

        public void CloseAllMenus()
        {
            foreach (var menu in MenuList.Where(menu => menu.Visible))
            {
                menu.Visible = false;
            }
        }
    }
}