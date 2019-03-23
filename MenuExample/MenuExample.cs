using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GTA;
using NativeUI;

public class MenuExample : Script
{
    private bool ketchup = false;
    private string dish = "Banana";
    private MenuPool _menuPool;

    public void AddMenuKetchup(UIMenu menu)
    {
        var newitem = new UIMenuCheckboxItem("Add ketchup?", ketchup, "Do you wish to add ketchup?");
        menu.AddItem(newitem);
        menu.OnCheckboxChange += (sender, item, checked_) =>
        {
            if (item == newitem)
            {
                ketchup = checked_;
                UI.Notify("~r~Ketchup status: ~b~" + ketchup);
            }
        };
    }

    public void AddMenuFoods(UIMenu menu)
    {
        var foods = new List<dynamic>
        {
            "Banana",
            "Apple",
            "Pizza",
            "Quartilicious",
            0xF00D, // Dynamic!
        };
        var newitem = new UIMenuListItem("Food", foods, 0);
        menu.AddItem(newitem);
        menu.OnListChange += (sender, item, index) =>
        {
            if (item == newitem)
            {
                dish = item.Items[index].ToString();
                UI.Notify("Preparing ~b~" + dish + "~w~...");
            }

        };
    }

    public void AddMenuCook(UIMenu menu)
    {
        var newitem = new UIMenuItem("Cook!", "Cook the dish with the appropiate ingredients and ketchup.");
        newitem.SetLeftBadge(UIMenuItem.BadgeStyle.Star);
        newitem.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
        menu.AddItem(newitem);
        menu.OnItemSelect += (sender, item, index) =>
        {
            if (item == newitem)
            {
                string output = ketchup ? "You have ordered ~b~{0}~w~ ~r~with~w~ ketchup." : "You have ordered ~b~{0}~w~ ~r~without~w~ ketchup.";
                UI.ShowSubtitle(String.Format(output, dish));
            }
        };
        menu.OnIndexChange += (sender, index) =>
        {
            if (sender.MenuItems[index] == newitem)
                newitem.SetLeftBadge(UIMenuItem.BadgeStyle.None);
        };
    }

    public void AddMenuAnotherMenu(UIMenu menu)
    {
        var submenu = _menuPool.AddSubMenu(menu, "Another Menu");
        for (int i = 0; i < 20; i++)
            submenu.AddItem(new UIMenuItem("PageFiller", "Sample description that takes more than one line. Moreso, it takes way more than two lines since it's so long. Wow, check out this length!"));
    }

    public MenuExample()
    {
        _menuPool = new MenuPool();
        var mainMenu = new UIMenu("Native UI", "~b~NATIVEUI SHOWCASE");
        _menuPool.Add(mainMenu);
        AddMenuKetchup(mainMenu);
        AddMenuFoods(mainMenu);
        AddMenuCook(mainMenu);
        AddMenuAnotherMenu(mainMenu);
        _menuPool.RefreshIndex();

        Tick += (o, e) => _menuPool.ProcessMenus();
        KeyDown += (o, e) =>
        {
            if (e.KeyCode == Keys.F5 && !_menuPool.IsAnyMenuOpen()) // Our menu on/off switch
                mainMenu.Visible = !mainMenu.Visible;
        };
    }
}
