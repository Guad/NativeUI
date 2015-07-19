using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using Control = GTA.Control;
using Font = GTA.Font;

namespace NativeUI
{
    public delegate void IndexChangedEvent(UIMenu sender, int newIndex);

    public delegate void ListChangedEvent(UIMenu sender, UIMenuListItem listItem, int newIndex);

    public delegate void CheckboxChangeEvent(UIMenu sender, UIMenuCheckboxItem checkboxItem, bool Checked);

    public delegate void ItemSelectEvent(UIMenu sender, UIMenuItem selectedItem, int index);


    /// <summary>
    /// Base class for NativeUI. Calls the next events: OnIndexChange, OnListChanged, OnCheckboxChange, OnItemSelect.
    /// </summary>
    public class UIMenu
    {
        private readonly UIContainer _mainMenu;
        private readonly Sprite _logo;

        private readonly UIRectangle _descriptionBar;
        private readonly UIRectangle _descriptionRectangle;
        private UIText _descriptionText;


        private int _activeItem = 1000;

        //Pagination
        private const int MaxItemsOnScreen = 12;
        private int _minItem;
        private int _maxItem = MaxItemsOnScreen;

        
        private readonly Sprite _upAndDownSprite;
        private readonly UIRectangle _extraRectangle;

        private Point Offset;
        
        public List<UIMenuItem> MenuItems = new List<UIMenuItem>();

        //Events

        /// <summary>
        /// Called when user presses up or down, changing current selection.
        /// </summary>
        public event IndexChangedEvent OnIndexChange;

        /// <summary>
        /// Called when user presses left or right, changing a list position.
        /// </summary>
        public event ListChangedEvent OnListChange;

        /// <summary>
        /// Called when user presses enter on a checkbox item.
        /// </summary>
        public event CheckboxChangeEvent OnCheckboxChange;

        /// <summary>
        /// Called when user selects a simple item.
        /// </summary>
        public event ItemSelectEvent OnItemSelect;

        //Keys
        public Keys KeyUp { get; set; }
        public Keys KeyDown { get; set; }
        public Keys KeyLeft { get; set; }
        public Keys KeyRight { get; set; }
        public Keys KeySelect { get; set; }



        /// <summary>
        /// Basic Menu constructor.
        /// </summary>
        /// <param name="title">Title that appears on the big banner.</param>
        /// <param name="subtitle">Subtitle that appears in capital letters in a small black bar.</param>
        public UIMenu(string title, string subtitle) : this(title, subtitle, new Point(0, 0), "commonmenu", "interaction_bgd")
        {
        }


        /// <summary>
        /// Basic Menu constructor with an offset.
        /// </summary>
        /// <param name="title">Title that appears on the big banner.</param>
        /// <param name="subtitle">Subtitle that appears in capital letters in a small black bar.</param>
        /// <param name="offset">Point object with X and Y data for offsets. Applied to all menu elements.</param>
        public UIMenu(string title, string subtitle, Point offset) : this(title, subtitle, offset, "commonmenu", "interaction_bgd")
        {
        }


        /// <summary>
        /// Advanced Menu constructor that allows custom title banner.
        /// </summary>
        /// <param name="title">Title that appears on the big banner. Set to "" if you are using a custom banner.</param>
        /// <param name="subtitle">Subtitle that appears in capital letters in a small black bar.</param>
        /// <param name="offset">Point object with X and Y data for offsets. Applied to all menu elements.</param>
        /// <param name="spriteLibrary">Sprite library name for the banner.</param>
        /// <param name="spriteName">Sprite name for the banner.</param>
        public UIMenu(string title, string subtitle, Point offset, string spriteLibrary, string spriteName)
        {

            Offset = offset;

            _mainMenu = new UIContainer(new Point(0 + Offset.X, 0 + Offset.Y), new Size(700, 500), Color.FromArgb(0, 0, 0, 0));
            _logo = new Sprite(spriteLibrary, spriteName, new Point(0 + Offset.X, 0 + Offset.Y), new Size(290, 75));
            _mainMenu.Items.Add(new UIText(title, new Point(145, 15), 1.15f, Color.White, Font.HouseScript, true));
            _mainMenu.Items.Add(new UIRectangle(new Point(0, 75), new Size(290, 25), Color.Black));
            _mainMenu.Items.Add(new UIText(subtitle, new Point(5, 78), 0.35f, Color.WhiteSmoke, 0, false));
            Title = title;
            Subtitle = subtitle;
            _upAndDownSprite = new Sprite("commonmenu", "shop_arrows_upanddown", new Point(130 + Offset.X, 90 + 25 * (MaxItemsOnScreen + 1) + Offset.Y), new Size(30, 30));
            _extraRectangle = new UIRectangle(new Point(0 + Offset.X, 90 + 25 * (MaxItemsOnScreen + 1) + Offset.Y), new Size(300, 30), Color.FromArgb(100, 0, 0, 0));

            _descriptionBar = new UIRectangle(new Point(Offset.X, 120), new Size(290, 5), Color.Black);
            _descriptionRectangle = new UIRectangle(new Point(Offset.X, 125), new Size(290, 30), Color.FromArgb(150, 0, 0, 0));
            _descriptionText = new UIText("Description", new Point(Offset.X + 5, 125), 0.33f, Color.FromArgb(255, 255, 255, 255), Font.ChaletLondon, false);

            KeyUp = Keys.NumPad8;
            KeyDown = Keys.NumPad2;
            KeyLeft = Keys.NumPad4;
            KeyRight = Keys.NumPad6;
            KeySelect = Keys.NumPad5;
        }


        /// <summary>
        /// Add an item to the menu.
        /// </summary>
        /// <param name="item">Item object to be added. Can be normal item, checkbox or list item.</param>
        public void AddItem(UIMenuItem item)
        {
            item.Offset = Offset;
            item.Position(MenuItems.Count * 25);
            MenuItems.Add(item);

            _descriptionBar.Position = new Point(Offset.X, 25 + _descriptionBar.Position.Y);
            _descriptionRectangle.Position = new Point(Offset.X, 25 + _descriptionRectangle.Position.Y);
            _descriptionText.Position = new Point(Offset.X + 5, _descriptionText.Position.Y + 25);
        }


        /// <summary>
        /// Remove an item at index n
        /// </summary>
        /// <param name="index">Index to remove the item at.</param>
        public void RemoveItemAt(int index)
        {
            MenuItems.RemoveAt(index);
            _descriptionBar.Position = new Point(Offset.X, _descriptionBar.Position.Y - 25);
            _descriptionRectangle.Position = new Point(Offset.X, _descriptionRectangle.Position.Y - 25);
            _descriptionText.Position = new Point(Offset.X + 5, _descriptionText.Position.Y - 25);
        }


        /// <summary>
        /// Reset the current selected item to 0. Use this after you add or remove items dynamically.
        /// </summary>
        public void RefreshIndex()
        {
            _activeItem = 1000 - (1000 % MenuItems.Count);
            _maxItem = MaxItemsOnScreen;
            _minItem = 0;
        }


        /// <summary>
        /// Remove all items from the menu.
        /// </summary>
        public void Clear()
        {
            MenuItems.Clear();
            _descriptionBar.Position = new Point(Offset.X, 125);
            _descriptionRectangle.Position = new Point(Offset.X, 130);
            _descriptionText.Position = new Point(Offset.X + 5, 130);
        }


        /// <summary>
        /// Draw the menu and all of it's components.
        /// </summary>
        public void Draw()
        {
            if (!Visible) return;
            _logo.Draw();
            MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
            _mainMenu.Draw();
            if (!String.IsNullOrWhiteSpace(MenuItems[_activeItem%(MenuItems.Count)].Description))
            {
                _descriptionText.Caption = FormatDescription(MenuItems[_activeItem%(MenuItems.Count)].Description);
                int numLines = _descriptionText.Caption.Split('\n').Length;
                _descriptionRectangle.Size = new Size(290, numLines * 20 + 2);

                _descriptionBar.Draw();
                _descriptionRectangle.Draw();
                _descriptionText.Draw();
            }
            if (MenuItems.Count <= MaxItemsOnScreen)
            {
                foreach (var item in MenuItems)
                {
                    item.Draw();
                }
            }
            else
            {
                int count = 0;
                for (int index = _minItem; index <= _maxItem; index++)
                {
                    var item = MenuItems[index];
                    item.Position(count * 25);
                    item.Draw();
                    count++;
                }
                _extraRectangle.Draw();
                _upAndDownSprite.Draw();
            }
        }


        /// <summary>
        /// Process keystroke. Call this in the OnKeyDown event.
        /// </summary>
        public void ProcessKey(Keys key)
        {
            if(!Visible) return;

            if (Game.IsControlJustPressed(0, Control.FrontendUp) || key == KeyUp)
            {
                if (_activeItem % MenuItems.Count <= _minItem)
                {
                    if (_activeItem % MenuItems.Count == 0)
                    {
                        _minItem = MenuItems.Count - MaxItemsOnScreen - 1;
                        _maxItem = MenuItems.Count - 1;
                        MenuItems[_activeItem % (MenuItems.Count)].Selected = false;
                        _activeItem = 1000 - (1000 % MenuItems.Count);
                        _activeItem += MenuItems.Count - 1;
                        MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
                    }
                    else
                    {
                        _minItem--;
                        _maxItem--;
                        MenuItems[_activeItem % (MenuItems.Count)].Selected = false;
                        _activeItem--;
                        MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
                    }
                }
                else
                {
                    MenuItems[_activeItem % (MenuItems.Count)].Selected = false;
                    _activeItem--;
                    MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
                }
                Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                IndexChange(CurrentSelection);
            }
            else if (Game.IsControlJustPressed(0, Control.FrontendDown) || key == KeyDown)
            {
                if (_activeItem % MenuItems.Count >= _maxItem)
                {
                    if (_activeItem % MenuItems.Count == MenuItems.Count - 1)
                    {
                        _minItem = 0;
                        _maxItem = MaxItemsOnScreen;
                        MenuItems[_activeItem % (MenuItems.Count)].Selected = false;
                        _activeItem = 1000 - (1000 % MenuItems.Count);
                        MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
                    }
                    else
                    {
                        _minItem++;
                        _maxItem++;
                        MenuItems[_activeItem % (MenuItems.Count)].Selected = false;
                        _activeItem++;
                        MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
                    }
                }
                else
                {
                    MenuItems[_activeItem % (MenuItems.Count)].Selected = false;
                    _activeItem++;
                    MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
                }
                Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                IndexChange(CurrentSelection);
            }
            else if (Game.IsControlJustPressed(0, Control.FrontendLeft) || key == KeyLeft)
            {
                if (!(MenuItems[CurrentSelection] is UIMenuListItem)) return;
                var it = (UIMenuListItem) MenuItems[CurrentSelection];
                it.Index--;
                Game.PlaySound("NAV_LEFT_RIGHT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                ListChange(it, it.Index);
            }

            else if (Game.IsControlJustPressed(0, Control.FrontendRight) || key == KeyRight)
            {
                if (!(MenuItems[CurrentSelection] is UIMenuListItem)) return;
                var it = (UIMenuListItem) MenuItems[CurrentSelection];
                it.Index++;
                Game.PlaySound("TOGGLE_ON", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                ListChange(it, it.Index);
            }

            else if (Game.IsControlJustPressed(0, Control.FrontendAccept) || key == KeySelect)
            {
                if (MenuItems[CurrentSelection] is UIMenuCheckboxItem)
                {
                    var it = (UIMenuCheckboxItem) MenuItems[CurrentSelection];
                    it.Checked = !it.Checked;
                    Game.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                    CheckboxChange(it, it.Checked);
                }
                else if (!(MenuItems[CurrentSelection] is UIMenuListItem))
                {
                    Game.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                    ItemSelect(MenuItems[CurrentSelection], CurrentSelection);
                }
            }
        }

        private string FormatDescription(string input)
        {
            int maxPixelsPerLine = 250;
            int aggregatePixels = 0;
            string output = "";
            string[] words = input.Split(' ');
            foreach (string word in words)
            {
                SizeF strSize;
                using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
                {
                    strSize = g.MeasureString(word, new System.Drawing.Font("Helvetica", 11, FontStyle.Regular, GraphicsUnit.Pixel));
                }
                aggregatePixels += Convert.ToInt32(strSize.Width);
                if (aggregatePixels > maxPixelsPerLine)
                {
                    output += "\n" + word + " ";
                    aggregatePixels = Convert.ToInt32(strSize.Width);
                }
                else
                {
                    output += word + " ";
                }
            }
            return output;
        }

        /// <summary>
        /// Change whether this menu is visible to the user.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Returns the current selected item's index.
        /// Change the current selected item to index. Use this after you add or remove items dynamically.
        /// </summary>
        public int CurrentSelection
        {
            get { return _activeItem % MenuItems.Count; }
            set
            {
                _activeItem = 1000 - (1000 % MenuItems.Count) + value;
                if (CurrentSelection > _maxItem)
                {
                    _maxItem = CurrentSelection;
                    _minItem = CurrentSelection - MaxItemsOnScreen;
                }
                else if (CurrentSelection < _minItem)
                {
                    _maxItem = MaxItemsOnScreen + CurrentSelection;
                    _minItem = CurrentSelection;
                }
            }
        }


        /// <summary>
        /// Returns the amount of items in the menu.
        /// </summary>
        public int Size
        {
            get { return MenuItems.Count; }
        }


        /// <summary>
        /// Returns the current title.
        /// </summary>
        public string Title { get; }


        /// <summary>
        /// Returns the current subtitle.
        /// </summary>
        public string Subtitle { get; }

        protected virtual void IndexChange(int newindex)
        {
            OnIndexChange?.Invoke(this, newindex);
        }

        protected virtual void ListChange(UIMenuListItem sender, int newindex)
        {
            OnListChange?.Invoke(this, sender, newindex);
        }

        protected virtual void ItemSelect(UIMenuItem selecteditem, int index)
        {
            OnItemSelect?.Invoke(this, selecteditem, index);
        }

        protected virtual void CheckboxChange(UIMenuCheckboxItem sender, bool Checked)
        {
            OnCheckboxChange?.Invoke(this, sender, Checked);
        }
    }
}

