using GTA;
using System;
using System.Windows.Forms;

namespace NativeUI
{
    /// <summary>
    /// Helper class to set up a script that has a menu.
    /// </summary>
    public abstract class UIScript : Script
    {
        private MenuPool pool = new MenuPool();

        /// <summary>
        /// Override this method to create and return the main menu.
        /// </summary>
        /// <returns></returns>
        public abstract UIMenu Menu();

        /// <summary>
        /// Override this method to change the script's activation key.
        /// Default implementation checks for F5.
        /// </summary>
        /// <param name="e">Key event.</param>
        /// <returns>Whether the script activation key is pressed or not.</returns>
        public bool IsScriptKeyPressed(KeyEventArgs e)
        {
            return (e.KeyCode == Keys.F5);
        }

        /// <summary>
        /// Add submenu to an existing menu.
        /// </summary>
        /// <param name="menu">The parent menu.</param>
        /// <param name="text">Text used for the item to be added to the parent menu.</param>
        /// <param name="OnMenuOpen">What to do when submenu is openend.</param>
        /// <param name="OnMenuClose">What to do when submenu is closed.</param>
        /// <param name="OnMenuHover">What to do when submenu is hovered.</param>
        /// <returns></returns>
        public UIMenu AddSubMenu(
            UIMenu menu, string text,
            Action OnMenuOpen = null, Action OnMenuClose = null, Action OnMenuHover = null)
        {
            var submenu = pool.AddSubMenu(menu, text);
            if (OnMenuOpen != null)
                submenu.ParentMenu.OnItemSelect += (sender, item, index) =>
                {
                    if (item == submenu.ParentItem) OnMenuOpen();
                };
            if (OnMenuClose != null)
                submenu.OnMenuClose += (sender) =>
                {
                    OnMenuClose();
                };
            if (OnMenuHover != null)
                submenu.ParentMenu.OnIndexChange += (sender, index) =>
                {
                    if (sender.MenuItems[index] == submenu.ParentItem) OnMenuHover();
                };
            return submenu;
        }

        /// <summary>
        /// Add item to menu.
        /// </summary>
        /// <param name="menu">Parent menu.</param>
        /// <param name="item">The item.</param>
        /// <param name="OnItemSelect">What to do when item is selected.</param>
        /// <param name="OnItemHover">What to do if item is hovered.</param>
        public void AddItem(
            UIMenu menu, UIMenuItem item,
            Action OnItemSelect = null, Action OnItemHover = null)
        {
            menu.AddItem(item);
            if (OnItemSelect != null)
                menu.OnItemSelect += (sender, item2, index) =>
                {
                    if (item == item2) OnItemSelect();
                };
            if (OnItemHover != null)
                menu.OnIndexChange += (sender, index) =>
                {
                    if (sender.MenuItems[index] == item) OnItemHover();
                };
        }

        /// <summary>
        /// Add list to menu.
        /// </summary>
        /// <param name="menu">Parent menu.</param>
        /// <param name="listitem">The list.</param>
        /// <param name="OnItemSelect">What to do when list is selected.</param>
        /// <param name="OnItemHover">What to do when list is hovered.</param>
        /// <param name="OnListChange">What to do when list selection is changed.</param>
        public void AddListItem(
            UIMenu menu, UIMenuListItem listitem,
            Action OnItemSelect = null, Action OnItemHover = null, Action<int> OnListChange = null)
        {
            AddItem(menu, listitem, OnItemSelect: OnItemSelect, OnItemHover: OnItemHover);
            if (OnListChange != null)
                menu.OnListChange += (sender, item, index) =>
                {
                    if (item == listitem) OnListChange(index);
                };
        }

        /// <summary>
        /// Add checkbox to menu.
        /// </summary>
        /// <param name="menu">Parent menu.</param>
        /// <param name="item">The checkbox item.</param>
        /// <param name="OnItemSelect">What to do when item is selected.</param>
        /// <param name="OnItemHover">What to do when item is hovered.</param>
        /// <param name="OnCheckboxChange">What to do when checkbox selection is changed.</param>
        public void AddCheckboxItem(
            UIMenu menu, UIMenuCheckboxItem item,
            Action OnItemSelect = null, Action OnItemHover = null, Action<bool> OnCheckboxChange = null)
        {
            AddItem(menu, item, OnItemSelect: OnItemSelect, OnItemHover: OnItemHover);
            if (OnCheckboxChange != null)
                menu.OnCheckboxChange += (sender, item2, checked_) =>
                {
                    if (item == item2) OnCheckboxChange(checked_);
                };
        }

        /// <summary>
        /// Set up script menu and register script key.
        /// </summary>
        public UIScript()
        {
            var menu = Menu();
            pool.Add(menu);
            pool.RefreshIndex();
            Tick += (sender, e) => pool.ProcessMenus();
            KeyUp += (sender, e) =>
            {
                if (IsScriptKeyPressed(e) && !pool.IsAnyMenuOpen())
                    menu.Visible = !menu.Visible;
            };
        }

    }
}
