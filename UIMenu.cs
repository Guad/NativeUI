using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
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

    public delegate void MenuCloseEvent(UIMenu sender);

    public delegate void MenuChangeEvent(UIMenu oldMenu, UIMenu newMenu, bool forward);

    public delegate void ItemActivatedEvent(UIMenu sender, UIMenuItem selectedItem);

    public delegate void ItemCheckboxEvent(UIMenuCheckboxItem sender, bool Checked);

    public delegate void ItemListEvent(UIMenuListItem sender, int newIndex);

    /// <summary>
    /// Base class for NativeUI. Calls the next events: OnIndexChange, OnListChanged, OnCheckboxChange, OnItemSelect.
    /// </summary>
    public class UIMenu
    {
        private readonly UIContainer _mainMenu;
        private Sprite _logo;
        private readonly Sprite _background;

        private readonly UIResRectangle _descriptionBar;
        private readonly Sprite _descriptionRectangle;
        private UIResText _descriptionText;
        private UIResText _counterText;

        private string _customBanner = null;

        private int _activeItem = 1000;

        private bool _visible;

        private bool _justOpened = true;

        //Pagination
        private const int MaxItemsOnScreen = 9;
        private int _minItem;
        private int _maxItem = MaxItemsOnScreen;

        
        private readonly Sprite _upAndDownSprite;
        private readonly UIResRectangle _extraRectangleUp;
        private readonly UIResRectangle _extraRectangleDown;

        private Point Offset;
        private int ExtraYOffset;
        
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

        /// <summary>
        /// Called when user closes the menu or goes back in a menu chain.
        /// </summary>
        public event MenuCloseEvent OnMenuClose;

        /// <summary>
        /// Called when user either clicks on a binded button or goes back to a parent menu.
        /// </summary>
        public event MenuChangeEvent OnMenuChange;

        //Keys
        private Dictionary<MenuControls, Tuple<List<Keys>, List<Tuple<GTA.Control, int>>>> _keyDictionary = new Dictionary<MenuControls, Tuple<List<Keys>, List<Tuple<GTA.Control, int>>>> ();


        //Tree structure
        public Dictionary<UIMenuItem, UIMenu> Children { get; private set; }
        
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

        public UIMenu(string title, string subtitle, Point offset, string customBanner) : this(title, subtitle, offset, "commonmenu", "interaction_bgd")
        {
            _customBanner = customBanner;
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
            Children = new Dictionary<UIMenuItem, UIMenu>();

            UpdateScaleform();

            _mainMenu = new UIContainer(new Point(0, 0), new Size(700, 500), Color.FromArgb(0, 0, 0, 0));
            _logo = new Sprite(spriteLibrary, spriteName, new Point(0 + Offset.X, 0 + Offset.Y), new Size(431, 107));
            _mainMenu.Items.Add(Title = new UIResText(title, new Point(215 + Offset.X, 20 + Offset.Y), 1.15f, Color.White, Font.HouseScript, true));
            if (!String.IsNullOrWhiteSpace(subtitle))
            {
                _mainMenu.Items.Add(new UIResRectangle(new Point(0 + offset.X, 107 + Offset.Y), new Size(431, 37), Color.Black));
                _mainMenu.Items.Add(Subtitle = new UIResText(subtitle, new Point(8 + Offset.X, 110 + Offset.Y), 0.35f, Color.WhiteSmoke, 0, false));

                if (subtitle.StartsWith("~"))
                {
                    CounterPretext = subtitle.Substring(0, 3);
                }
                _counterText = new UIResText("", new Point(360 + Offset.X, 110 + Offset.Y), 0.35f, Color.WhiteSmoke, 0, false);
                ExtraYOffset = 37;
            }

            _upAndDownSprite = new Sprite("commonmenu", "shop_arrows_upanddown", new Point(190 + Offset.X, 147 + 37 * (MaxItemsOnScreen + 1) + Offset.Y - 37 + ExtraYOffset), new Size(50, 50));
            _extraRectangleUp = new UIResRectangle(new Point(0 + Offset.X, 144 + 38 * (MaxItemsOnScreen + 1) + Offset.Y - 37 + ExtraYOffset), new Size(431, 18), Color.FromArgb(200, 0, 0, 0));
            _extraRectangleDown = new UIResRectangle(new Point(0 + Offset.X, 144 + 18 + 38 * (MaxItemsOnScreen + 1) + Offset.Y - 37 + ExtraYOffset), new Size(431, 18), Color.FromArgb(200, 0, 0, 0));

            _descriptionBar = new UIResRectangle(new Point(Offset.X, 123), new Size(431, 4), Color.Black);
            _descriptionRectangle = new Sprite("commonmenu", "gradient_bgd", new Point(Offset.X, 127), new Size(431, 30));
            _descriptionText = new UIResText("Description", new Point(Offset.X + 5, 125), 0.35f, Color.FromArgb(255, 255, 255, 255), Font.ChaletLondon, false);

            _background = new Sprite("commonmenu", "gradient_bgd", new Point(Offset.X, 144 + Offset.Y - 37 + ExtraYOffset), new Size(290, 25));

            SetKey(MenuControls.Up, GTA.Control.FrontendUp);
            SetKey(MenuControls.Down, GTA.Control.FrontendDown);
            SetKey(MenuControls.Left, GTA.Control.FrontendLeft);
            SetKey(MenuControls.Right, GTA.Control.FrontendRight);
            SetKey(MenuControls.Select, GTA.Control.FrontendAccept);

            SetKey(MenuControls.Back, GTA.Control.FrontendCancel);
            SetKey(MenuControls.Back, GTA.Control.FrontendPause);
            SetKey(MenuControls.Back, GTA.Control.PhoneCancel);
        }

        private void RecaulculateDescriptionPosition()
        {
            _descriptionBar.Position = new Point(Offset.X, 149 - 37 + ExtraYOffset + Offset.Y);
            _descriptionRectangle.Position = new Point(Offset.X, 149 - 37 + ExtraYOffset + Offset.Y);
            _descriptionText.Position = new Point(Offset.X + 8, 155 - 37 + ExtraYOffset + Offset.Y);

            int count = Size;
            if (count > MaxItemsOnScreen + 1)
                count = MaxItemsOnScreen + 2;

            _descriptionBar.Position = new Point(Offset.X, 38*count + _descriptionBar.Position.Y);
            _descriptionRectangle.Position = new Point(Offset.X, 38*count + _descriptionRectangle.Position.Y);
            _descriptionText.Position = new Point(Offset.X + 8, 38*count + _descriptionText.Position.Y);
        }

        private void DisEnableControls(bool enable)
        {
            Hash thehash = enable ? Hash.ENABLE_CONTROL_ACTION : Hash.DISABLE_CONTROL_ACTION;
            foreach (var con in Enum.GetValues(typeof(GTA.Control)))
            {
                Function.Call(thehash, 0, (int)con);
                Function.Call(thehash, 1, (int)con);
                Function.Call(thehash, 2, (int)con);
            }
            //Controls we want
            // -Frontend
            // -Mouse
            // -Walk/Move
            // -
            
            if (!enable)
            {
                var list = new List<GTA.Control>
                {
                    Control.FrontendAccept,
                    Control.FrontendAxisX,
                    Control.FrontendAxisY,
                    Control.FrontendDown,
                    Control.FrontendUp,
                    Control.FrontendLeft,
                    Control.FrontendRight,
                    Control.FrontendCancel,
                    Control.FrontendSelect,
                    Control.CursorScrollDown,
                    Control.CursorScrollUp,
                    Control.CursorX,
                    Control.CursorY,
                    Control.MoveUpDown,
                    Control.MoveLeftRight,
                    Control.Sprint,
                    Control.Jump,
                    Control.Enter,
                    Control.VehicleExit,
                    Control.VehicleAccelerate,
                    Control.VehicleBrake,
                    Control.VehicleMoveLeftRight,
                    Control.VehicleFlyYawLeft,
                    Control.FlyLeftRight,
                    Control.FlyUpDown,
                    Control.VehicleFlyYawRight,
                    Control.VehicleHandbrake,
                };
                foreach (var control in list)
                {
                    Function.Call(Hash.ENABLE_CONTROL_ACTION, 0, (int)control);
                } 
            }
        }

        private bool buttonsEnabled = false;

        public void EnableInstructionalButtons(bool enable)
        {
            buttonsEnabled = enable;
        }

        /// <summary>
        /// Set the banner to your own Sprite.
        /// </summary>
        /// <param name="spriteBanner">Sprite object. The position and size does not matter.</param>
        public void SetBannerType(Sprite spriteBanner)
        {
            //_logo = new Sprite(spriteLibrary, spriteName, new Point(0 + Offset.X, 0 + Offset.Y), new Size(431, 107));
            _logo = spriteBanner;
            _logo.Size = new Size(431, 107);
            _logo.Position = new Point(Offset.X, Offset.Y);
        }

        
        private UIResRectangle _tmpRectangle;
        /// <summary>
        ///  Set the banner to your own Rectangle.
        /// </summary>
        /// <param name="rectangle">UIResRectangle object. Position and size does not matter.</param>
        public void SetBannerType(UIResRectangle rectangle)
        {
            //_logo = new Sprite(spriteLibrary, spriteName, new Point(0 + Offset.X, 0 + Offset.Y), new Size(431, 107));
            _logo = null;
            _tmpRectangle = rectangle;
            _tmpRectangle.Position = new Point(Offset.X, Offset.Y);
            _tmpRectangle.Size = new Size(431, 107);
        }

        /// <summary>
        /// Set the banner to your own custom sprite.
        /// </summary>
        /// <param name="pathToCustomSprite">Path to your sprite image.</param>
        public void SetBannerType(string pathToCustomSprite)
        {
            _customBanner = pathToCustomSprite;
        }

        /// <summary>
        /// Add an item to the menu.
        /// </summary>
        /// <param name="item">Item object to be added. Can be normal item, checkbox or list item.</param>
        public void AddItem(UIMenuItem item)
        {
            item.Offset = Offset;
            item.Position((MenuItems.Count * 25) - 37 + ExtraYOffset);
            item.Parent = this;
            MenuItems.Add(item);

            RecaulculateDescriptionPosition();
        }


        /// <summary>
        /// Remove an item at index n
        /// </summary>
        /// <param name="index">Index to remove the item at.</param>
        public void RemoveItemAt(int index)
        {
            if (Size > MaxItemsOnScreen && _maxItem == Size - 1)
            {
                _maxItem--;
                _minItem--;
            }
            MenuItems.RemoveAt(index);
            RecaulculateDescriptionPosition();
        }


        /// <summary>
        /// Reset the current selected item to 0. Use this after you add or remove items dynamically.
        /// </summary>
        public void RefreshIndex()
        {
            if (MenuItems.Count == 0)
            {
                _activeItem = 1000;
                _maxItem = MaxItemsOnScreen;
                _minItem = 0;
                return;
            }
            MenuItems[_activeItem % (MenuItems.Count)].Selected = false;
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
            RecaulculateDescriptionPosition();
        }
        
        /// <summary>
        /// Draw the menu and all of it's components.
        /// </summary>
        public void Draw()
        {
            if (!Visible) return;

            DisEnableControls(false);
            if(buttonsEnabled)
                _instructionalButtonsScaleform.Render2D();
            
            Function.Call((Hash)0xB8A850F20A067EB6, 76, 84);           // Safezone
            Function.Call((Hash)0xF5A2C681787E579D, 0f, 0f, 0f, 0f);   // stuff

            _background.Size = Size > MaxItemsOnScreen + 1 ? new Size(431, 38*(MaxItemsOnScreen + 1)) : new Size(431, 38 * Size);
            _background.Draw();

            if (String.IsNullOrWhiteSpace(_customBanner))
            {
                if (_logo != null)
                    _logo.Draw();
                else if (_tmpRectangle != null)
                    _tmpRectangle.Draw();
            }
            else
            {
                // TODO: DrawTexture
                //UI.DrawTexture(_customBanner, 1, 1, 40, new Point(Offset.X, Offset.Y), new Size(290, 75));
                int safeX;
                int safeY;
                GetSafezoneBounds(out safeX, out safeY);
                Sprite.DrawTexture(_customBanner, new Point(safeX + Offset.X, safeY + Offset.Y), new Size(431, 107));
            }
            MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
            _mainMenu.Draw();
            if (!String.IsNullOrWhiteSpace(MenuItems[_activeItem%(MenuItems.Count)].Description))
            {
                _descriptionText.Caption = FormatDescription(MenuItems[_activeItem%(MenuItems.Count)].Description);
                int numLines = _descriptionText.Caption.Split('\n').Length;
                _descriptionRectangle.Size = new Size(431, (numLines * 25) + 15);

                _descriptionBar.Draw();
                _descriptionRectangle.Draw();
                _descriptionText.Draw();
            }

            if (MenuItems.Count <= MaxItemsOnScreen + 1)
            {
                int count = 0;
                foreach (var item in MenuItems)
                {
                    item.Position(count * 38 - 37 + ExtraYOffset);
                    item.Draw();
                    count++;
                }
            }
            else
            {
                int count = 0;
                for (int index = _minItem; index <= _maxItem; index++)
                {
                    var item = MenuItems[index];
                    item.Position(count * 38 - 37 + ExtraYOffset);
                    item.Draw();
                    count++;
                }
                _extraRectangleUp.Draw();
                _extraRectangleDown.Draw();
                _upAndDownSprite.Draw();
                if (_counterText != null)
                {
                    string cap = (CurrentSelection + 1) + " / " + Size;
                    Function.Call((Hash)0x54CE8AC98E120CAB, "jamyfafi");
                    UIResText.AddLongString(cap);
                    int screenw = Game.ScreenResolution.Width;
                    int screenh = Game.ScreenResolution.Height;
                    const float height = 1080f;
                    float ratio = (float)screenw / screenh;
                    var width = height * ratio;
                    int offset = Convert.ToInt32(Function.Call<float>((Hash)0x85F061DA64ED2F67, (int)0) * width * 0.35f);
                    _counterText.Position = new Point(430 - offset + Offset.X, 110 + Offset.Y);
                    _counterText.Caption = CounterPretext + cap;
                    _counterText.Draw();
                }
            }
            Function.Call((Hash)0xE3A3DB414A373DAB); // Safezone end
        }

        public SizeF GetScreenResolutionMantainRatio()
        {
            int screenw = Game.ScreenResolution.Width;
            int screenh = Game.ScreenResolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;

            return new SizeF(width, height);
        }

        public bool IsMouseInBounds(Point TopLeft, Size boxSize)
        {
            var res = GetScreenResolutionMantainRatio();

            int mouseX = Convert.ToInt32(Math.Round(Function.Call<float>(Hash.GET_CONTROL_NORMAL, 0, (int)GTA.Control.CursorX) * res.Width));
            int mouseY = Convert.ToInt32(Math.Round(Function.Call<float>(Hash.GET_CONTROL_NORMAL, 0, (int)GTA.Control.CursorY) * res.Height));

            return (mouseX >= TopLeft.X && mouseX <= TopLeft.X + boxSize.Width)
                   && (mouseY > TopLeft.Y && mouseY < TopLeft.Y + boxSize.Height);
        }
        

        /// <summary>
        /// Function to get whether the cursor is in an arrow space, or in label.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>0 - Not in item at all, 1 - In label, 2 - In arrow space.</returns>
        public int IsMouseInListItemArrows(UIMenuListItem item, Point TopLeft, Point Safezone)
        {
            Function.Call((Hash)0x54CE8AC98E120CAB, "jamyfafi");
            UIResText.AddLongString(item.Text);
            var res = GetScreenResolutionMantainRatio();
            var screenw = res.Width;
            var screenh = res.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;
            int labelSize = Convert.ToInt32(Function.Call<float>((Hash)0x85F061DA64ED2F67, (int)0) * width * 0.35f);

            int labelSizeX = 5 + labelSize + 10;
            int arrowSizeX = 431 - labelSizeX;
            return IsMouseInBounds(TopLeft, new Size(labelSizeX, 38))
                ? 1
                : IsMouseInBounds(new Point(TopLeft.X + labelSizeX, TopLeft.Y), new Size(arrowSizeX, 38)) ? 2 : 0;

        }


        public void GetSafezoneBounds(out int safezoneX, out int safezoneY)
        {
            float t = Function.Call<float>(Hash._0xBAF107B6BB2C97F0); // Safezone size.
            double g = Math.Round(Convert.ToDouble(t), 2);
            g = (g * 100) - 90;
            g = 10 - g;

            const float hmp = 5.4f;
            int screenw = Game.ScreenResolution.Width;
            int screenh = Game.ScreenResolution.Height;
            float ratio = (float)screenw / screenh;
            float wmp = ratio*hmp;
            
            safezoneX = Convert.ToInt32(Math.Round(g*wmp));
            safezoneY = Convert.ToInt32(Math.Round(g*hmp));
        }


        public void GoUpOverflow()
        {
            if (Size <= MaxItemsOnScreen + 1) return;
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


        public void GoUp()
        {
            if (Size > MaxItemsOnScreen + 1) return;
            MenuItems[_activeItem % (MenuItems.Count)].Selected = false;
            _activeItem--;
            MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
            Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
            IndexChange(CurrentSelection);
        }


        public void GoDownOverflow()
        {
            if (Size <= MaxItemsOnScreen + 1) return;
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


        public void GoDown()
        {
            if (Size > MaxItemsOnScreen + 1) return;
            MenuItems[_activeItem % (MenuItems.Count)].Selected = false;
            _activeItem++;
            MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
            Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
            IndexChange(CurrentSelection);
        }


        public void GoLeft()
        {
            if (!(MenuItems[CurrentSelection] is UIMenuListItem)) return;
            var it = (UIMenuListItem)MenuItems[CurrentSelection];
            it.Index--;
            Game.PlaySound("NAV_LEFT_RIGHT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
            ListChange(it, it.Index);
            it.ListChangedTrigger(it.Index);
        }


        public void GoRight()
        {
            if (!(MenuItems[CurrentSelection] is UIMenuListItem)) return;
            var it = (UIMenuListItem)MenuItems[CurrentSelection];
            it.Index++;
            Game.PlaySound("NAV_LEFT_RIGHT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
            ListChange(it, it.Index);
            it.ListChangedTrigger(it.Index);
        }


        public void SelectItem()
        {
            if (MenuItems[CurrentSelection] is UIMenuCheckboxItem)
            {
                var it = (UIMenuCheckboxItem)MenuItems[CurrentSelection];
                it.Checked = !it.Checked;
                Game.PlaySound("OK", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                CheckboxChange(it, it.Checked);
                it.CheckboxEventTrigger();
            }
            else
            {
                Game.PlaySound("OK", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                ItemSelect(MenuItems[CurrentSelection], CurrentSelection);
                MenuItems[CurrentSelection].ItemActivate(this);
                if (!Children.ContainsKey(MenuItems[CurrentSelection])) return;
                Visible = false;
                Children[MenuItems[CurrentSelection]].Visible = true;
                MenuChangeEv(Children[MenuItems[CurrentSelection]], true);
            }
        }


        public void GoBack()
        {
            Game.PlaySound("BACK", "HUD_FRONTEND_DEFAULT_SOUNDSET");
            Visible = false;
            if (ParentMenu != null)
            {
                ParentMenu.Visible = true;
                MenuChangeEv(ParentMenu, false);
            }
            MenuCloseEv();
        }

        /// <summary>
        /// Bind a menu to a button. When the button is clicked that menu will open.
        /// </summary>
        public void BindMenuToItem(UIMenu menuToBind, UIMenuItem itemToBindTo)
        {
            menuToBind.ParentMenu = this;
            menuToBind.ParentItem = itemToBindTo;
            if (Children.ContainsKey(itemToBindTo))
                Children[itemToBindTo] = menuToBind;
            else
                Children.Add(itemToBindTo, menuToBind);
        }


        /// <summary>
        /// Remove menu binding from button.
        /// </summary>
        /// <param name="releaseFrom">Button to release from.</param>
        /// <returns>Returns true if the operation was successful.</returns>
        public bool ReleaseMenuFromItem(UIMenuItem releaseFrom)
        {
            if (!Children.ContainsKey(releaseFrom)) return false;
            Children[releaseFrom].ParentItem = null;
            Children[releaseFrom].ParentMenu = null;
            Children.Remove(releaseFrom);
            return true;
        }


        /// <summary>
        /// Remove menu binding from button with specific menu.
        /// </summary>
        /// <param name="menuToRelease">What menu to release</param>
        /// <param name="releaseFrom">Button to release from.</param>
        /// <returns>Returns true if the operation was successful.</returns>
        public bool ReleaseMenuFromItem(UIMenuItem releaseFrom, UIMenu menuToRelease)
        {
            if (!Children.ContainsKey(releaseFrom) || Children[releaseFrom] != menuToRelease) return false;
            menuToRelease.ParentItem = null;
            menuToRelease.ParentMenu = null;
            Children.Remove(releaseFrom);
            return true;
        }

        /// <summary>
        /// Call this in OnTick
        /// </summary>
        public void ProcessMouse()
        {
            if (!Visible || _justOpened) return;
            int safezoneOffsetX;
            int safezoneOffsetY;
            GetSafezoneBounds(out safezoneOffsetX, out safezoneOffsetY);
            Function.Call(Hash._SHOW_CURSOR_THIS_FRAME);
            int limit = MenuItems.Count - 1;
            int counter = 0;
            if (MenuItems.Count > MaxItemsOnScreen + 1)
                limit = _maxItem;

            for (int i = _minItem; i <= limit; i++)
            {
                int Xpos = Offset.X + safezoneOffsetX;
                int Ypos = Offset.Y + 144 - 37 + ExtraYOffset + (counter*38) + safezoneOffsetY;
                int Xsize = 431;
                int Ysize = 38;
                UIMenuItem uiMenuItem = MenuItems[i];
                if (IsMouseInBounds(new Point(Xpos, Ypos), new Size(Xsize, Ysize)))
                {
                    uiMenuItem.Hovered = true;
                    if (Game.IsControlJustPressed(0, GTA.Control.Attack))
                        if (uiMenuItem.Selected)
                        {
                            if (MenuItems[i] is UIMenuListItem &&
                                IsMouseInListItemArrows((UIMenuListItem) MenuItems[i], new Point(Xpos, Ypos),
                                    new Point(safezoneOffsetX, safezoneOffsetY)) > 0)
                            {
                                int res = IsMouseInListItemArrows((UIMenuListItem) MenuItems[i], new Point(Xpos, Ypos),
                                    new Point(safezoneOffsetX, safezoneOffsetY));
                                switch (res)
                                {
                                    case 1:
                                        Game.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                                        MenuItems[i].ItemActivate(this);
                                        ItemSelect(MenuItems[i], i);
                                        break;
                                    case 2:
                                        var it = (UIMenuListItem) MenuItems[i];
                                        it.Index++;
                                        Game.PlaySound("NAV_LEFT_RIGHT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                                        ListChange(it, it.Index);
                                        it.ListChangedTrigger(it.Index);
                                        break;
                                }
                            }
                            else
                                SelectItem();
                        }
                        else
                        {
                            CurrentSelection = i;
                            Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                            IndexChange(CurrentSelection);
                            UpdateScaleform();
                        }
                }
                else
                    uiMenuItem.Hovered = false;
                counter++;
            }
            int extraY = 144 + 38*(MaxItemsOnScreen + 1) + Offset.Y - 37 + ExtraYOffset + safezoneOffsetY;
            int extraX = safezoneOffsetX + Offset.X;
            if(Size <= MaxItemsOnScreen + 1) return;
            if (IsMouseInBounds(new Point(extraX, extraY), new Size(431, 18)))
            {
                _extraRectangleUp.Color = Color.FromArgb(255, 30, 30, 30);
                if (Game.IsControlJustPressed(0, GTA.Control.Attack))
                {
                    if(Size > MaxItemsOnScreen+1)
                        GoUpOverflow();
                    else
                        GoUp();
                }
            }
            else
                _extraRectangleUp.Color = Color.FromArgb(200, 0, 0, 0);
            
            if (IsMouseInBounds(new Point(extraX, extraY+18), new Size(431, 18)))
            {
                _extraRectangleDown.Color = Color.FromArgb(255, 30, 30, 30);
                if (Game.IsControlJustPressed(0, GTA.Control.Attack))
                {
                    if (Size > MaxItemsOnScreen + 1)
                        GoDownOverflow();
                    else
                        GoDown();
                }
            }
            else
                _extraRectangleDown.Color = Color.FromArgb(200, 0, 0, 0);
        }


        /// <summary>
        /// Set a key to control a menu. Can be multiple keys for each control.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="keyToSet"></param>
        public void SetKey(MenuControls control, Keys keyToSet)
        {
            if (_keyDictionary.ContainsKey(control))
                _keyDictionary[control].Item1.Add(keyToSet);
            else
            {
                _keyDictionary.Add(control,
                    new Tuple<List<Keys>, List<Tuple<Control, int>>>(new List<Keys>(), new List<Tuple<Control, int>>()));
                _keyDictionary[control].Item1.Add(keyToSet);
            }
        }


        /// <summary>
        /// Set a GTA.Control to control a menu. Can be multiple controls. This applies it to all indexes.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="gtaControl"></param>
        public void SetKey(MenuControls control, GTA.Control gtaControl)
        {
            SetKey(control, gtaControl, 0);
            SetKey(control, gtaControl, 1);
            SetKey(control, gtaControl, 2);
        }


        /// <summary>
        /// Set a GTA.Control to control a menu only on a specific index.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="gtaControl"></param>
        /// <param name="controlIndex"></param>
        public void SetKey(MenuControls control, GTA.Control gtaControl, int controlIndex)
        {
            if (_keyDictionary.ContainsKey(control))
                _keyDictionary[control].Item2.Add(new Tuple<Control, int>(gtaControl, controlIndex));
            else
            {
                _keyDictionary.Add(control,
                    new Tuple<List<Keys>, List<Tuple<Control, int>>>(new List<Keys>(), new List<Tuple<Control, int>>()));
                _keyDictionary[control].Item2.Add(new Tuple<Control, int>(gtaControl, controlIndex));
            }

        }


        /// <summary>
        /// Remove all controls on a control.
        /// </summary>
        /// <param name="control"></param>
        public void ResetKey(MenuControls control)
        {
            _keyDictionary[control].Item1.Clear();
            _keyDictionary[control].Item2.Clear();
        }


        public bool HasControlJustBeenPressed(MenuControls control, Keys key = Keys.None)
        {
            List<Keys> tmpKeys = new List<Keys>(_keyDictionary[control].Item1);
            List<Tuple<GTA.Control, int>> tmpControls = new List<Tuple<Control, int>>(_keyDictionary[control].Item2);

            if (key != Keys.None)
            {
                if (tmpKeys.Any(Game.IsKeyPressed))
                    return true;
            }
            if (tmpControls.Any(tuple => Game.IsControlJustPressed(tuple.Item2, tuple.Item1)))
                return true;
            return false;
        }


        public bool HasControlJustBeenReleaseed(MenuControls control, Keys key = Keys.None)
        {
            List<Keys> tmpKeys = new List<Keys>(_keyDictionary[control].Item1);
            List<Tuple<GTA.Control, int>> tmpControls = new List<Tuple<Control, int>>(_keyDictionary[control].Item2);

            if (key != Keys.None)
            {
                if (tmpKeys.Any(Game.IsKeyPressed))
                    return true;
            }
            if (tmpControls.Any(tuple => Game.IsControlJustReleased(tuple.Item2, tuple.Item1)))
                return true;
            return false;
        }
        

        /// <summary>
        /// Process control-stroke. Call this in the OnTick event.
        /// </summary>
        public void ProcessControl(Keys key = Keys.None)
        {
            if(!Visible) return;
            if (_justOpened)
            {
                _justOpened = false;
                return;
            }
            if (HasControlJustBeenPressed(MenuControls.Up, key) || Game.IsControlJustPressed(0, GTA.Control.CursorScrollUp))
            {
                //MenuPool.ControllerUsed = Game.IsControlJustPressed(2, (GTA.Control)27);
                MenuPool.ControllerUsed = Game.IsControlJustPressed(2, GTA.Control.PhoneUp);
                if (Size > MaxItemsOnScreen + 1)
                    GoUpOverflow();
                else
                    GoUp();
                UpdateScaleform();
            }
            else if (HasControlJustBeenPressed(MenuControls.Down, key) || Game.IsControlJustPressed(0, GTA.Control.CursorScrollDown))
            {
                //MenuPool.ControllerUsed = Game.IsControlJustPressed(2, (GTA.Control)8);
                MenuPool.ControllerUsed = Game.IsControlJustPressed(2, GTA.Control.PhoneDown);

                if (Size > MaxItemsOnScreen + 1)
                    GoDownOverflow();
                else
                    GoDown();
                UpdateScaleform();
            }
            else if (HasControlJustBeenPressed(MenuControls.Left, key))
            {
                //MenuPool.ControllerUsed = Game.IsControlJustPressed(2, (GTA.Control)34);
                MenuPool.ControllerUsed = Game.IsControlJustPressed(2, GTA.Control.PhoneLeft);
                GoLeft();
            }

            else if (HasControlJustBeenPressed(MenuControls.Right, key))
            {
                //MenuPool.ControllerUsed = Game.IsControlJustPressed(2, (GTA.Control)9);
                MenuPool.ControllerUsed = Game.IsControlJustPressed(2, GTA.Control.PhoneRight);
                GoRight();
            }

            else if (HasControlJustBeenPressed(MenuControls.Select, key))
            {
                //MenuPool.ControllerUsed = Game.IsControlJustPressed(2, (GTA.Control)18);
                SelectItem();
            }

            else if (HasControlJustBeenReleaseed(MenuControls.Back, key))
            {
                //MenuPool.ControllerUsed = Game.IsControlJustPressed(2, (GTA.Control)45);
                GoBack();
            }
        }


        /// <summary>
        /// Process keystroke. Call this in the OnKeyDown event.
        /// </summary>
        /// <param name="key"></param>
        public void ProcessKey(Keys key)
        {
            if ((from object menuControl in Enum.GetValues(typeof(MenuControls)) select new List<Keys>(_keyDictionary[(MenuControls)menuControl].Item1)).Any(tmpKeys => tmpKeys.Any(k => k == key)))
            {
                ProcessControl(key);
            }
        }


        private string FormatDescription(string input)
        {
            int maxPixelsPerLine = 425;
            int aggregatePixels = 0;
            string output = "";
            string[] words = input.Split(' ');
            foreach (string word in words)
            {
                Function.Call((Hash)0x54CE8AC98E120CAB, "jamyfafi");
                UIResText.AddLongString(word);
                int screenw = Game.ScreenResolution.Width;
                int screenh = Game.ScreenResolution.Height;
                const float height = 1080f;
                float ratio = (float)screenw / screenh;
                var width = height * ratio;
                int offset = Convert.ToInt32(Function.Call<float>((Hash)0x85F061DA64ED2F67, (int)0) * width * 0.35f);

                aggregatePixels += Convert.ToInt32(offset);
                if (aggregatePixels > maxPixelsPerLine)
                {
                    output += "\n" + word + " ";
                    aggregatePixels = Convert.ToInt32(offset);
                }
                else
                {
                    output += word + " ";
                }
            }
            return output;
        }


        private Dictionary<dynamic, string> _scaleformButtons = new Dictionary<dynamic, string>();

        public void AddButtonToScaleform(string button, string text)
        {
            if (_scaleformButtons.ContainsKey(button))
                _scaleformButtons[button] = text;
            else
                _scaleformButtons.Add(button, text);
        }

        public void AddButtonToScaleform(GTA.Control button, string text)
        {
            if (_scaleformButtons.ContainsKey(button))
                _scaleformButtons[button] = text;
            else
                _scaleformButtons.Add(button, text);
        }

        public void RemoveButtonFromScaleform(GTA.Control button)
        {
            if (_scaleformButtons.ContainsKey(button))
                _scaleformButtons.Remove(button);
        }

        public void RemoveButtonFromScaleform(string button)
        {
            if (_scaleformButtons.ContainsKey(button))
                _scaleformButtons.Remove(button);
        }

        private Dictionary<UIMenuItem, Tuple<dynamic, string>> _itemScaleformButtons = new Dictionary<UIMenuItem, Tuple<dynamic, string>>();

        public void AddButtonToScaleform(string button, string text, UIMenuItem itemToBindto)
        {
            if (_itemScaleformButtons.ContainsKey(itemToBindto))
            {
                _itemScaleformButtons[itemToBindto] = new Tuple<dynamic, string>(button, text);
            }
            else
            {
                _itemScaleformButtons.Add(itemToBindto, new Tuple<dynamic, string>(button, text));
            }
        }

        public void AddButtonToScaleform(GTA.Control button, string text, UIMenuItem itemToBindto)
        {
            if (_itemScaleformButtons.ContainsKey(itemToBindto))
            {
                _itemScaleformButtons[itemToBindto] = new Tuple<dynamic, string>(button, text);
            }
            else
            {
                _itemScaleformButtons.Add(itemToBindto, new Tuple<dynamic, string>(button, text));
            }
        }

        public void RemoveButtonFromScaleform(UIMenuItem item)
        {
            if (_itemScaleformButtons.ContainsKey(item))
                _itemScaleformButtons.Remove(item);
        }

        private Scaleform _instructionalButtonsScaleform;
        public void UpdateScaleform()
        {
            _instructionalButtonsScaleform = new Scaleform(0);
            _instructionalButtonsScaleform.Load("instructional_buttons");

            _instructionalButtonsScaleform.CallFunction("CLEAR_ALL");
            _instructionalButtonsScaleform.CallFunction("CLEAR_RENDER");
            //instructionalButtonsScaleform.CallFunction("SET_DATA_SLOT_EMPTY");
            if (!Visible) return;
            _instructionalButtonsScaleform.CallFunction("SET_DISPLAY_CONFIG", 1280, 720, 0.05, 0.95, 0.05, 0.95, true, false, false,
                1365.33, 768);
            _instructionalButtonsScaleform.CallFunction("SET_MAX_WIDTH", 1);
            //instructionalButtonsScaleform.CallFunction("TOGGLE_MOUSE_BUTTONS", 1);
            _instructionalButtonsScaleform.CallFunction("CREATE_CONTAINER");

            _instructionalButtonsScaleform.CallFunction("SET_DATA_SLOT", 0, Function.Call<string>(Hash._0x0499D7B09FC9B407, 2, (int)GTA.Control.PhoneSelect, 0), "Select");
            _instructionalButtonsScaleform.CallFunction("SET_DATA_SLOT", 1, Function.Call<string>(Hash._0x0499D7B09FC9B407, 2, (int)GTA.Control.PhoneCancel, 0), "Back");
            int count = 2;
            foreach (var button in _scaleformButtons)
            {
                _instructionalButtonsScaleform.CallFunction("SET_DATA_SLOT", count,
                    button.Key.GetType() == typeof(GTA.Control)
                        ? Function.Call<string>(Hash._0x0499D7B09FC9B407, 2, (int)button.Key, 0)
                        : "t_" + button.Key, button.Value);

                count++;
            }
            int count2 = count + 1;
            foreach (var scaleformButton in _itemScaleformButtons)
            {
                if (MenuItems[CurrentSelection] == scaleformButton.Key)
                {
                    _instructionalButtonsScaleform.CallFunction("SET_DATA_SLOT", count,
                    scaleformButton.Value.Item1.GetType() == typeof(GTA.Control)
                        ? Function.Call<string>(Hash._0x0499D7B09FC9B407, 2, (int)scaleformButton.Value.Item1, 0)
                        : "t_" + scaleformButton.Value.Item1, scaleformButton.Value.Item2);
                    count2++;
                }
            }
            _instructionalButtonsScaleform.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", 0);
            //instructionalButtonsScaleform.CallFunction("FLASH_BUTTON_BY_ID", 31, 100, 1);
        }

        /// <summary>
        /// Change whether this menu is visible to the user.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {   
                _visible = value;
                _justOpened = value;
                UpdateScaleform();
            }
        }


        /// <summary>
        /// Returns the current selected item's index.
        /// Change the current selected item to index. Use this after you add or remove items dynamically.
        /// </summary>
        public int CurrentSelection
        {
            get { return _activeItem % MenuItems.Count; }
            set
            {
                MenuItems[_activeItem%(MenuItems.Count)].Selected = false;
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
        public UIResText Title { get; }


        /// <summary>
        /// Returns the current subtitle.
        /// </summary>
        public UIResText Subtitle { get; }


        public string CounterPretext { get; set; }

       
        public UIMenu ParentMenu { get; set; }

        
        public UIMenuItem ParentItem { get; set; }


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
        
        protected virtual void MenuCloseEv()
        {
            OnMenuClose?.Invoke(this);
        }

        protected virtual void MenuChangeEv(UIMenu newmenu, bool forward)
        {
            OnMenuChange?.Invoke(this, newmenu, forward);
        }

        public enum MenuControls
        {
            Up,
            Down,
            Left,
            Right,
            Select,
            Back
        }
    }
}
