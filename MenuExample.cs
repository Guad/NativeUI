using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using NativeUI;

public class MenuExample : Script
{
    private UIMenu mainMenu;
    private UIMenu newMenu;
    private UIMenuCheckboxItem ketchupCheckbox;
    private UIMenuListItem dishesListItem;
    private UIMenuItem cookItem;

    private MenuPool _menuPool;

    public MenuExample()
    {
        Tick += OnTick;
        KeyDown += OnKeyDown;
        _menuPool = new MenuPool();

        mainMenu = new UIMenu("Native UI", "~b~NATIVEUI SHOWCASE");
        _menuPool.Add(mainMenu);

        mainMenu.AddItem(ketchupCheckbox = new UIMenuCheckboxItem("Add ketchup?", false, "Do you wish to add ketchup?"));
        var foods = new List<dynamic>
        {
            "Banana",
            "Apple",
            "Pizza",
            "Quartilicious",
            0xF00D, // Dynamic!
        };
        mainMenu.AddItem(dishesListItem = new UIMenuListItem("Food", foods, 0));
        mainMenu.AddItem(cookItem = new UIMenuItem("Cook!", "Cook the dish with the appropiate ingredients and ketchup."));

        var menuItem = new UIMenuItem("Go to another menu.");
        mainMenu.AddItem(menuItem);
        cookItem.SetLeftBadge(UIMenuItem.BadgeStyle.Star);
        cookItem.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
        mainMenu.RefreshIndex();

        mainMenu.OnItemSelect += OnItemSelect;
        mainMenu.OnListChange += OnListChange;
        mainMenu.OnCheckboxChange += OnCheckboxChange;
        mainMenu.OnIndexChange += OnItemChange;

        newMenu = new UIMenu("Native UI", "~r~NATIVEUI SHOWCASE");
        _menuPool.Add(newMenu);
        for (int i = 0; i < 20; i++)
        {
            newMenu.AddItem(new UIMenuItem("PageFiller", "Sample description that takes more than one line. Moreso, it takes way more than two lines since it's so long. Wow, check out this length!"));
        }
        newMenu.RefreshIndex();
        mainMenu.BindMenuToItem(newMenu, menuItem);
        
    }

    public void OnItemChange(UIMenu sender, int index)
    {
        sender.MenuItems[index].SetLeftBadge(UIMenuItem.BadgeStyle.None);
    }

    public void OnCheckboxChange(UIMenu sender, UIMenuCheckboxItem checkbox, bool Checked)
    {
        if (sender != mainMenu || checkbox != ketchupCheckbox) return; // We only want to detect changes from our menu.
        UI.Notify("~r~Ketchup status: ~b~" + Checked);
    }

    public void OnListChange(UIMenu sender, UIMenuListItem list, int index)
    {
        if (sender != mainMenu || list != dishesListItem) return; // We only want to detect changes from our menu.
        string dish = list.IndexToItem(index).ToString();
        UI.Notify("Preparing ~b~" + dish +"~w~...");
    }
    
    public void OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
    {
        if (sender != mainMenu || selectedItem != cookItem) return; // We only want to detect changes from our menu and our button.
        // You can also detect the button by using index
        string dish = dishesListItem.IndexToItem(dishesListItem.Index).ToString();
        bool ketchup = ketchupCheckbox.Checked;

        string output = ketchup
            ? "You have ordered ~b~{0}~w~ ~r~with~w~ ketchup."
            : "You have ordered ~b~{0}~w~ ~r~without~w~ ketchup.";
        UI.ShowSubtitle(String.Format(output, dish));
    }

    public void OnTick(object o, EventArgs e)
    {
        _menuPool.ProcessMenus();
    }

    public void OnKeyDown(object o, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F5 && !_menuPool.IsAnyMenuOpen()) // Our menu on/off switch
        {
            mainMenu.Visible = !mainMenu.Visible;
        }
    }
}
