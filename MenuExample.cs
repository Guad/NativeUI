using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using NativeUI;

public class MenuExample : UIScript
{
    private bool ketchup = false;
    private string dish = "Banana";

    public void AddMenuKetchup(UIMenu menu)
    {
        var newitem = new UIMenuCheckboxItem("Add ketchup?", ketchup, "Do you wish to add ketchup?");
        Action<bool> KetchupStatus = (checked_) =>
        {
            ketchup = checked_;
            UI.Notify("~r~Ketchup status: ~b~" + ketchup);
        };
        AddCheckboxItem(menu, newitem, OnChange: KetchupStatus);
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
        Action<int> PrepareDish = (index) =>
        {
            dish = newitem.IndexToItem(index).ToString();
            UI.Notify("Preparing ~b~" + dish + "~w~...");
        };
        AddListItem(menu, newitem, OnChange: PrepareDish);
    }

    public void AddMenuCook(UIMenu menu)
    {
        var newitem = new UIMenuItem("Cook!", "Cook the dish with the appropiate ingredients and ketchup.");
        newitem.SetLeftBadge(UIMenuItem.BadgeStyle.Star);
        newitem.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
        Action Cook = () =>
        {
            string output = ketchup ? "You have ordered ~b~{0}~w~ ~r~with~w~ ketchup." : "You have ordered ~b~{0}~w~ ~r~without~w~ ketchup.";
            UI.ShowSubtitle(string.Format(output, dish));
        };
        Action RemoveStar = () =>
        {
            newitem.SetLeftBadge(UIMenuItem.BadgeStyle.None);
        };
        AddItem(menu, newitem, OnSelect: Cook, OnHover: RemoveStar);
    }

    public void AddMenuAnotherMenu(UIMenu menu)
    {
        var submenu = AddSubMenu(menu, "Another Menu");
        for (int i = 0; i < 20; i++)
            AddItem(submenu, new UIMenuItem("PageFiller", "Sample description that takes more than one line. Moreso, it takes way more than two lines since it's so long. Wow, check out this length!"));
    }

    public override UIMenu Menu()
    {
        var menu = new UIMenu("Native UI", "~b~NATIVEUI SHOWCASE");
        AddMenuKetchup(menu);
        AddMenuFoods(menu);
        AddMenuCook(menu);
        AddMenuAnotherMenu(menu);
        return menu;
    }
}
