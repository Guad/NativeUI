using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GTA;

namespace NativeUI
{
    public delegate void OnIndexChanged(UIMenu sender, int newIndex);

    public delegate void OnListChanged(UIMenu sender, UIMenuListItem listItem, int newIndex);

    public delegate void OnCheckboxChange(UIMenu sender, UIMenuCheckboxItem checkboxItem, bool Checked);

    public delegate void OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index);

    public class UIMenu
    {
        /// <summary>
        /// Base class for NativeUI. Calls the next events:
        /// - OnIndexChanged - Called when user presses up or down, changing current selection.
        /// - OnListChanged - Called when user presses left or right, changing a list position.
        /// - OnCheckboxChange - Called when user presses enter on a checkbox item.
        /// - OnItemSelect - Called when user selects a simple item.
        /// </summary>
        
        private readonly UIContainer _mainMenu;
        private readonly Sprite logo;

        private int _activeItem = 1000;

        //Pagination
        private const int MaxItemsOnScreen = 12;
        private int _minItem = 0;
        private int _maxItem = MaxItemsOnScreen;

        
        private readonly Sprite upAndDownSprite;
        private readonly UIRectangle extraRectangle;

        private Point Offset;

        //old
        //public List<dynamic> MenuItems = new List<dynamic>();
        public List<UIMenuItem> MenuItems = new List<UIMenuItem>();

        //Events
        public event OnIndexChanged IndexChange;
        public event OnListChanged ListChange;
        public event OnCheckboxChange CheckboxChange;
        public event OnItemSelect ItemSelect;

        public UIMenu(string title, string subtitle) : this(title, subtitle, new Point(0, 0), "commonmenu", "interaction_bgd")
        {
        }

        public UIMenu(string title, string subtitle, Point offset) : this(title, subtitle, offset, "commonmenu", "interaction_bgd")
        {
        }

        public UIMenu(string title, string subtitle, Point offset, string spriteLibrary, string spriteName)
        {

            Offset = offset;

            _mainMenu = new UIContainer(new Point(0 + Offset.X, 0 + Offset.Y), new Size(700, 500), Color.FromArgb(0, 0, 0, 0));
            logo = new Sprite(spriteLibrary, spriteName, new Point(0 + Offset.X, 0 + Offset.Y), new Size(300, 60));
            _mainMenu.Items.Add(new UIText(title, new Point(150, 5), 1.15f, Color.White, GTA.Font.HouseScript, true));
            _mainMenu.Items.Add(new UIRectangle(new Point(0, 60), new Size(300, 30), Color.Black));
            _mainMenu.Items.Add(new UIText(subtitle.ToUpper(), new Point(10, 66), 0.3f, Color.WhiteSmoke, 0, false));
            Title = subtitle;
            upAndDownSprite = new Sprite("commonmenu", "shop_arrows_upanddown", new Point(130 + Offset.X, 90 + 25 * (MaxItemsOnScreen + 1) + Offset.Y), new Size(30, 30));
            extraRectangle = new UIRectangle(new Point(0 + Offset.X, 90 + 25 * (MaxItemsOnScreen + 1) + Offset.Y), new Size(300, 30), Color.FromArgb(100, 0, 0, 0));
        }

        public void AddItem(UIMenuItem item)
        {
            item.Offset = Offset;
            item.Position(MenuItems.Count * 25);
            MenuItems.Add(item);
        }

        public void RemoveItemAt(int index)
        {
            MenuItems.RemoveAt(index);
        }

        public void RefreshIndex()
        {
            _activeItem = 1000 - (1000 % MenuItems.Count);
            _maxItem = MaxItemsOnScreen;
            _minItem = 0;
        }

        public void SetIndex(int newindex)
        {
            _activeItem = 1000 - (1000 % MenuItems.Count) + newindex;
            if (CurrentSelection > _maxItem)
            {
                _maxItem = CurrentSelection;
                _minItem = CurrentSelection - MaxItemsOnScreen;
            }
            else if(CurrentSelection < _minItem)
            {
                _maxItem = MaxItemsOnScreen + CurrentSelection;
                _minItem = CurrentSelection;
            }
        }

        public void Clear()
        {
            MenuItems.Clear();
        }


        public void Draw()
        {
            if (!Visible) return;
            logo.Draw();
            MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
            _mainMenu.Draw();
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
                extraRectangle.Draw();
                upAndDownSprite.Draw();
            }
        }

        public void ProcessKey(Keys key)
        {
            if (Game.IsControlJustPressed(0, GTA.Control.FrontendUp) || Game.IsControlJustPressed(1, GTA.Control.FrontendUp) || Game.IsControlJustPressed(2, GTA.Control.FrontendUp))
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
                OnIndexChange(CurrentSelection);
            }
            else if (Game.IsControlJustPressed(0, GTA.Control.FrontendDown) || Game.IsControlJustPressed(1, GTA.Control.FrontendDown) || Game.IsControlJustPressed(2, GTA.Control.FrontendDown))
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
                OnIndexChange(CurrentSelection);
            }
            else if (Game.IsControlJustPressed(0, GTA.Control.FrontendLeft) || Game.IsControlJustPressed(1, GTA.Control.FrontendLeft) || Game.IsControlJustPressed(2, GTA.Control.FrontendLeft))
            {
                if (!(MenuItems[CurrentSelection] is UIMenuListItem)) return;
                var it = (UIMenuListItem) MenuItems[CurrentSelection];
                it.Index--;
                OnListChange(it, it.Index);
            }

            else if (Game.IsControlJustPressed(0, GTA.Control.FrontendRight) || Game.IsControlJustPressed(1, GTA.Control.FrontendRight) || Game.IsControlJustPressed(2, GTA.Control.FrontendRight))
            {
                if (!(MenuItems[CurrentSelection] is UIMenuListItem)) return;
                var it = (UIMenuListItem) MenuItems[CurrentSelection];
                it.Index++;
                OnListChange(it, it.Index);
            }

            else if (Game.IsControlJustPressed(0, GTA.Control.FrontendSelect) || Game.IsControlJustPressed(1, GTA.Control.FrontendSelect) || Game.IsControlJustPressed(2, GTA.Control.FrontendSelect))
            {
                if (MenuItems[CurrentSelection] is UIMenuCheckboxItem)
                {
                    var it = (UIMenuCheckboxItem) MenuItems[CurrentSelection];
                    it.Checked = !it.Checked;
                    OnCheckboxChange(it, it.Checked);
                }
                else if (!(MenuItems[CurrentSelection] is UIMenuListItem))
                {
                    OnItemSelect(MenuItems[CurrentSelection], CurrentSelection);
                }
            }
        }

        public bool Visible { get; set; }

        public int CurrentSelection
        {
            get { return _activeItem % MenuItems.Count; }
        }

        public int Size
        {
            get { return MenuItems.Count; }
        }

        public string Title { get; set; }

        protected virtual void OnIndexChange(int newindex)
        {
            IndexChange?.Invoke(this, newindex);
        }

        protected virtual void OnListChange(UIMenuListItem sender, int newindex)
        {
            ListChange?.Invoke(this, sender, newindex);
        }

        protected virtual void OnItemSelect(UIMenuItem selecteditem, int index)
        {
            ItemSelect?.Invoke(this, selecteditem, index);
        }

        protected virtual void OnCheckboxChange(UIMenuCheckboxItem sender, bool Checked)
        {
            CheckboxChange?.Invoke(this, sender, Checked);
        }
    }
}

