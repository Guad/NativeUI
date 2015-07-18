using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.Math;
using NativeUI;

public class MenuExample : Script
{
    private UIMenu mainMenu;

    public MenuExample()
    {
        Tick += OnTick;
        KeyDown += OnKeyDown;

        mainMenu = new UIMenu("Showcase", "NativeUI Showcase");
        mainMenu.AddItem(new UIMenuCheckboxItem("Add ketchup?", false));
        var foods = new List<dynamic>
        {
            "Banana",
            "Apple",
            "Pizza",
            0xF00D, // Dynamic!
        };
        mainMenu.AddItem(new UIMenuListItem("Food", foods, 0));
        mainMenu.AddItem(new UIMenuItem("Cook!"));

        mainMenu.ItemSelect += ItemSelect;
    }

    public void CheckboxChange(UIMenu sender, UIMenuCheckboxItem checkbox, bool Checked)
    {
        if (sender != mainMenu) return; // We only want to detect changes from our menu.
        UI.Notify("~r~Ketchup status: ~b~" + Checked);
    }

    public void ListChange(UIMenu sender, UIMenuListItem list, int index)
    {
        if (sender != mainMenu) return; // We only want to detect changes from our menu.
        string dish = list.IndexToItem(index);
        UI.Notify("Preparing ~b~" + dish +"~w~...");
    }
    
    public void ItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
    {
        if (sender != mainMenu || selectedItem.Text != "Cook!") return; // We only want to detect changes from our menu and our button.
        // You can also detect the button by using index
        var ourList = (UIMenuListItem) sender.MenuItems[1];
        string dish = ourList.IndexToItem(ourList.Index);
        var ourCheckbox = (UIMenuCheckboxItem) sender.MenuItems[0];
        bool ketchup = ourCheckbox.Checked;

        string output = ketchup
            ? "You have ordered ~b~{0}~w~ ~r~with~w~ ketchup."
            : "You have ordered ~b~{0}~w~ ~r~without~w~ ketchup.";
        UI.ShowSubtitle(String.Format(output, dish));
    }

    public void OnTick(object o, EventArgs e)
    {
        mainMenu.Draw();
    }

    public void OnKeyDown(object o, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F5) // Our menu on/off switch
        {
            mainMenu.Visible = !mainMenu.Visible;
        }
    }
}
