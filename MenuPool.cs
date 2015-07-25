using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace NativeUI
{
    /// <summary>
    /// Helper class that handles all of your Menus. After instatiating it, you will have to add your menu by using the Add method.
    /// </summary>
    public class MenuPool
    {
        public static bool ControllerUsed { get; set; } //Not used

        private readonly List<UIMenu> _menuList = new List<UIMenu>();


        /// <summary>
        /// Add your menu to the menu pool.
        /// </summary>
        /// <param name="menu"></param>
        public void Add(UIMenu menu)
        {
            _menuList.Add(menu);
        }

        /// <summary>
        /// Returns all of your menus.
        /// </summary>
        /// <returns></returns>
        public List<UIMenu> ToList()
        {
            return _menuList;
        }

        /// <summary>
        /// Processes all of your visible menus' controls.
        /// </summary>
        public void ProcessControl()
        {
            foreach (var menu in _menuList.Where(menu => menu.Visible))
            {
                menu.ProcessControl();
            }
        }


        /// <summary>
        /// Processes all of your visible menus' keys.
        /// </summary>
        /// <param name="key"></param>
        public void ProcessKey(Keys key)
        {
            foreach (var menu in _menuList.Where(menu => menu.Visible))
            {
                menu.ProcessKey(key);
            }
        }


        /// <summary>
        /// Processes all of your visible menus' mouses.
        /// </summary>
        public void ProcessMouse()
        {
            foreach (var menu in _menuList.Where(menu => menu.Visible))
            {
                menu.ProcessMouse();
            }
        }
        

        /// <summary>
        /// Draws all visible menus.
        /// </summary>
        public void Draw()
        {
            foreach (var menu in _menuList.Where(menu => menu.Visible))
            {
                menu.Draw();
            }
        }


        /// <summary>
        /// Checks if any menu is currently visible.
        /// </summary>
        /// <returns>true if at least one menu is visible, false if not.</returns>
        public bool IsAnyMenuOpen()
        {
            return _menuList.Any(menu => menu.Visible);
        }


        /// <summary>
        /// Process all of your menus' functions. Call this in a tick event.
        /// </summary>
        public void ProcessMenus()
        {
            ProcessControl();
            ProcessMouse();
            Draw();
        }


        /// <summary>
        /// Closes all of your menus.
        /// </summary>
        public void CloseAllMenus()
        {
            foreach (var menu in _menuList.Where(menu => menu.Visible))
            {
                menu.Visible = false;
            }
        }
    }
}