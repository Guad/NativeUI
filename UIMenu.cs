using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using Control = GTA.Control;
using Font = GTA.UI.Font;	 
using Screen = GTA.UI.Screen;

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
    /// Base class for NativeUI. Calls the next events: OnIndexChange, OnListChanged, OnCheckboxChange, OnItemSelect, OnMenuClose, OnMenuchange.
    /// </summary>
    public class UIMenu
    {
        private readonly GTA.UI.ContainerElement _mainMenu;
        private Sprite _logo;
        private readonly Sprite _background;

        private readonly UIResRectangle _descriptionBar;
        private readonly Sprite _descriptionRectangle;
        private readonly UIResText _descriptionText;
        private readonly UIResText _counterText;

        private string _customBanner;

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

        private Point _offset;
        private readonly int _extraYOffset;

        public string AUDIO_LIBRARY = "HUD_FRONTEND_DEFAULT_SOUNDSET";

        public string AUDIO_UPDOWN = "NAV_UP_DOWN";
        public string AUDIO_LEFTRIGHT = "NAV_LEFT_RIGHT";
        public string AUDIO_SELECT = "SELECT";
        public string AUDIO_BACK = "BACK";
        public string AUDIO_ERROR = "ERROR";
        
        public List<UIMenuItem> MenuItems = new List<UIMenuItem>();

        public bool MouseEdgeEnabled = true;
        public bool ControlDisablingEnabled = true;
        public bool ResetCursorOnOpen = true;
        public bool FormatDescriptions = true;
        public bool MouseControlsEnabled = true;
        public bool ScaleWithSafezone = true;

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
        private readonly Dictionary<MenuControls, Tuple<List<Keys>, List<Tuple<Control, int>>>> _keyDictionary = new Dictionary<MenuControls, Tuple<List<Keys>, List<Tuple<Control, int>>>> ();
                         
        //Tree structure
        public Dictionary<UIMenuItem, UIMenu> Children { get; }
        
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
        /// <param name="subtitle">Subtitle that appears in capital letters in a small black bar. Set to "" if you dont want a subtitle.</param>
        /// <param name="offset">Point object with X and Y data for offsets. Applied to all menu elements.</param>
        public UIMenu(string title, string subtitle, Point offset) : this(title, subtitle, offset, "commonmenu", "interaction_bgd")
        {
        }

        /// <summary>
        /// Initialise a menu with a custom texture banner.
        /// </summary>
        /// <param name="title">Title that appears on the big banner. Set to "" if you don't want a title.</param>
        /// <param name="subtitle">Subtitle that appears in capital letters in a small black bar. Set to "" if you dont want a subtitle.</param>
        /// <param name="offset">Point object with X and Y data for offsets. Applied to all menu elements.</param>
        /// <param name="customBanner">Path to your custom texture.</param>
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
            _offset = offset;
            Children = new Dictionary<UIMenuItem, UIMenu>();
            WidthOffset = 0;

            _instructionalButtonsScaleform = new Scaleform("instructional_buttons");
            
            UpdateScaleform();

            _mainMenu = new GTA.UI.ContainerElement(new Point(0, 0), new Size(700, 500), Color.FromArgb(0, 0, 0, 0));
            _logo = new Sprite(spriteLibrary, spriteName, new Point(0 + _offset.X, 0 + _offset.Y), new Size(431, 107));
            _mainMenu.Items.Add(Title = new UIResText(title, new Point(215 + _offset.X, 20 + _offset.Y), 1.15f, Color.White, Font.HouseScript, UIResText.Alignment.Centered));
            if (!String.IsNullOrWhiteSpace(subtitle))
            {
                _mainMenu.Items.Add(new UIResRectangle(new Point(0 + offset.X, 107 + _offset.Y), new Size(431, 37), Color.Black));
                _mainMenu.Items.Add(Subtitle = new UIResText(subtitle, new Point(8 + _offset.X, 110 + _offset.Y), 0.35f, Color.WhiteSmoke, 0, UIResText.Alignment.Left));

                if (subtitle.StartsWith("~"))
                {
                    CounterPretext = subtitle.Substring(0, 3);
                }
                _counterText = new UIResText("", new Point(425 + _offset.X, 110 + _offset.Y), 0.35f, Color.WhiteSmoke, 0, UIResText.Alignment.Right);
                _extraYOffset = 37;
            }

            _upAndDownSprite = new Sprite("commonmenu", "shop_arrows_upanddown", new Point(190 + _offset.X, 147 + 37 * (MaxItemsOnScreen + 1) + _offset.Y - 37 + _extraYOffset), new Size(50, 50));
            _extraRectangleUp = new UIResRectangle(new Point(0 + _offset.X, 144 + 38 * (MaxItemsOnScreen + 1) + _offset.Y - 37 + _extraYOffset), new Size(431, 18), Color.FromArgb(200, 0, 0, 0));
            _extraRectangleDown = new UIResRectangle(new Point(0 + _offset.X, 144 + 18 + 38 * (MaxItemsOnScreen + 1) + _offset.Y - 37 + _extraYOffset), new Size(431, 18), Color.FromArgb(200, 0, 0, 0));

            _descriptionBar = new UIResRectangle(new Point(_offset.X, 123), new Size(431, 4), Color.Black);
            _descriptionRectangle = new Sprite("commonmenu", "gradient_bgd", new Point(_offset.X, 127), new Size(431, 30));
            _descriptionText = new UIResText("Description", new Point(_offset.X + 5, 125), 0.35f, Color.FromArgb(255, 255, 255, 255), Font.ChaletLondon, UIResText.Alignment.Left);

            _background = new Sprite("commonmenu", "gradient_bgd", new Point(_offset.X, 144 + _offset.Y - 37 + _extraYOffset), new Size(290, 25));

            
            SetKey(MenuControls.Up, Control.PhoneUp);
            SetKey(MenuControls.Up, Control.CursorScrollUp);

            SetKey(MenuControls.Down, Control.PhoneDown);
            SetKey(MenuControls.Down, Control.CursorScrollDown);

            SetKey(MenuControls.Left, Control.PhoneLeft);
            SetKey(MenuControls.Right, Control.PhoneRight);
            SetKey(MenuControls.Select, Control.FrontendAccept);

            SetKey(MenuControls.Back, Control.PhoneCancel);
            SetKey(MenuControls.Back, Control.FrontendPause);
        }

        private void RecaulculateDescriptionPosition()
        {
            //_descriptionText.WordWrap = new Size(425 + WidthOffset, 0);

            _descriptionBar.Position = new Point(_offset.X, 149 - 37 + _extraYOffset + _offset.Y);
            _descriptionRectangle.Position = new Point(_offset.X, 149 - 37 + _extraYOffset + _offset.Y);
            _descriptionText.Position = new Point(_offset.X + 8, 155 - 37 + _extraYOffset + _offset.Y);

            _descriptionBar.Size = new Size(431 + WidthOffset, 4);
            _descriptionRectangle.Size = new Size(431 + WidthOffset, 30);

            int count = Size;
            if (count > MaxItemsOnScreen + 1)
                count = MaxItemsOnScreen + 2;

            _descriptionBar.Position = new Point(_offset.X, (int)(38*count + _descriptionBar.Position.Y));
            _descriptionRectangle.Position = new Point(_offset.X, 38*count + _descriptionRectangle.Position.Y);
            _descriptionText.Position = new Point(_offset.X + 8, (int)(38*count + _descriptionText.Position.Y));
        }


        /// <summary>
        /// Returns the current width offset.
        /// </summary>
        public int WidthOffset { get; private set; }


        /// <summary>
        /// Change the menu's width. The width is calculated as DefaultWidth + WidthOffset, so a width offset of 10 would enlarge the menu by 10 pixels.
        /// </summary>
        /// <param name="widthOffset">New width offset.</param>
        public void SetMenuWidthOffset(int widthOffset)
        {
            WidthOffset = widthOffset;
            _logo.Size = new Size(431 + WidthOffset, 107);
            _mainMenu.Items[0].Position = new Point((WidthOffset + _offset.X + 431)/2, 20 + _offset.Y); // Title
            _counterText.Position = new Point(425 + _offset.X + widthOffset, 110 + _offset.Y);
            if (_mainMenu.Items.Count >= 1)
            {
                var tmp = (UIResRectangle) _mainMenu.Items[1];
                tmp.Size = new Size(431 + WidthOffset, 37);
            }
            if (_tmpRectangle != null)
            {
                _tmpRectangle.Size = new Size(431 + WidthOffset, 107);
            }
        }
        

        /// <summary>
        /// Enable or disable all controls but the necessary to operate a menu.
        /// </summary>
        /// <param name="enable"></param>
        public static void DisEnableControls(bool enable)
        {
            Hash thehash = enable ? Hash.ENABLE_CONTROL_ACTION : Hash.DISABLE_CONTROL_ACTION;
            foreach (var con in Enum.GetValues(typeof(Control)))
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

            if (enable) return;
            var list = new List<Control>
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

            if (IsUsingController)
            {
                list.AddRange(new Control[]
                {
                    Control.LookUpDown,
                    Control.LookLeftRight,
                    Control.Aim,
                    Control.Attack,
                });   
            }

            foreach (var control in list)
            {
                Function.Call(Hash.ENABLE_CONTROL_ACTION, 0, (int)control);
            }
        }
               
        private bool _buttonsEnabled = true;
        /// <summary>
        /// Enable or disable the instructional buttons.
        /// </summary>
        /// <param name="disable"></param>
        public void DisableInstructionalButtons(bool disable)
        {
            _buttonsEnabled = !disable;
        }
            

        /// <summary>
        /// Set the banner to your own Sprite object.
        /// </summary>
        /// <param name="spriteBanner">Sprite object. The position and size does not matter.</param>
        public void SetBannerType(Sprite spriteBanner)
        {
            _logo = spriteBanner;
            _logo.Size = new Size(431 + WidthOffset, 107);
            _logo.Position = new Point(_offset.X, _offset.Y);
        }
                   
        private UIResRectangle _tmpRectangle;
        /// <summary>
        ///  Set the banner to your own Rectangle.
        /// </summary>
        /// <param name="rectangle">UIResRectangle object. Position and size does not matter.</param>
        public void SetBannerType(UIResRectangle rectangle)
        {
            _logo = null;
            _tmpRectangle = rectangle;
            _tmpRectangle.Position = new Point(_offset.X, _offset.Y);
            _tmpRectangle.Size = new Size(431 + WidthOffset, 107);
        }


        /// <summary>
        /// Set the banner to your own custom texture. Set it to "" if you want to restore the banner.
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
            item.Offset = _offset;
            item.Parent = this;
            item.Position((MenuItems.Count * 25) - 37 + _extraYOffset);
            MenuItems.Add(item);

            RecaulculateDescriptionPosition();
        }


        /// <summary>
        /// Insert an item into a specific position.
        /// </summary>
        /// <param name="item">Item object to add.</param>
        /// <param name="position">Position in the list to insert at. Ranging from 0 to menu size - 1.</param>
        public void InsertItem(UIMenuItem item, int position)
        {
            item.Offset = _offset;
            item.Parent = this;
            item.Position((MenuItems.Count * 25) - 37 + _extraYOffset);
            MenuItems.Insert(position, item);

            RecaulculateDescriptionPosition();
        }


        /// <summary>
        /// Remove an item at index n.
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

            if(ControlDisablingEnabled)
                DisEnableControls(false);

            if(_buttonsEnabled)
                Function.Call(Hash.DRAW_SCALEFORM_MOVIE_FULLSCREEN, _instructionalButtonsScaleform.Handle, 255, 255, 255, 255, 0);
                // _instructionalButtonsScaleform.Render2D(); // Bug #13


            Point safe = GetSafezoneBounds();

            if (ScaleWithSafezone)
            {
                Function.Call((Hash) 0xB8A850F20A067EB6, 76, 84); // Safezone
                Function.Call((Hash) 0xF5A2C681787E579D, 0f, 0f, 0f, 0f); // stuff
            }
            else
            {
                safe = new Point(0, 0);
            }

            if (String.IsNullOrWhiteSpace(_customBanner))
            {
                if (_logo != null)
                    _logo.Draw();
                else
                {
                    _tmpRectangle?.Draw();
                }
            }
            else
            {
                Sprite.DrawTexture(_customBanner, new Point(safe.X + _offset.X, safe.Y + _offset.Y), new Size(431 + WidthOffset, 107));
            }
            _mainMenu.Draw();
            if (MenuItems.Count == 0)
            {
                Function.Call((Hash)0xE3A3DB414A373DAB); // Safezone end
                return;
            }

            _background.Size = Size > MaxItemsOnScreen + 1 ? new Size(431 + WidthOffset, 38*(MaxItemsOnScreen + 1)) : new Size(431 + WidthOffset, 38 * Size);
            _background.Draw();
            MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
            if (!String.IsNullOrWhiteSpace(MenuItems[_activeItem%(MenuItems.Count)].Description))
            {
                RecaulculateDescriptionPosition();
                string descCaption = MenuItems[_activeItem % (MenuItems.Count)].Description;
                if (FormatDescriptions)
                    _descriptionText.Caption = FormatDescription(descCaption);
                else
                    _descriptionText.Caption = descCaption;
                int numLines = _descriptionText.Caption.Split('\n').Length;
                _descriptionRectangle.Size = new Size(431 + WidthOffset, (numLines * 25) + 15);

                _descriptionBar.Draw();
                _descriptionRectangle.Draw();
                _descriptionText.Draw();
            }

            if (MenuItems.Count <= MaxItemsOnScreen + 1)
            {
                int count = 0;
                foreach (var item in MenuItems)
                {
                    item.Position(count * 38 - 37 + _extraYOffset);
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
                    item.Position(count * 38 - 37 + _extraYOffset);
                    item.Draw();
                    count++;
                }
                _extraRectangleUp.Size = new Size(431 + WidthOffset, 18);
                _extraRectangleDown.Size = new Size(431 + WidthOffset, 18);
                _upAndDownSprite.Position = new Point(190 + _offset.X + (WidthOffset/2),
                    147 + 37*(MaxItemsOnScreen + 1) + _offset.Y - 37 + _extraYOffset);

                _extraRectangleUp.Draw();
                _extraRectangleDown.Draw();
                _upAndDownSprite.Draw();
                if (_counterText != null)
                {
                    string cap = (CurrentSelection + 1) + " / " + Size;
                    _counterText.Caption = CounterPretext + cap;
                    _counterText.Draw();
                }
            }

            if (ScaleWithSafezone)
                Function.Call((Hash)0xE3A3DB414A373DAB); // Safezone end
        }

        /// <summary>
        /// Returns the 1080pixels-based screen resolution while mantaining current aspect ratio.
        /// </summary>
        /// <returns></returns>
        public static SizeF GetScreenResolutionMantainRatio()
        {
            int screenw = Screen.Resolution.Width;
            int screenh = Screen.Resolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;

            return new SizeF(width, height);
        }


        /// <summary>
        /// Chech whether the mouse is inside the specified rectangle.
        /// </summary>
        /// <param name="topLeft">top left point of your rectangle.</param>
        /// <param name="boxSize">size of your rectangle.</param>
        /// <returns></returns>
        public static bool IsMouseInBounds(Point topLeft, Size boxSize)
        {
            var res = GetScreenResolutionMantainRatio();

            int mouseX = Convert.ToInt32(Math.Round(Function.Call<float>(Hash.GET_CONTROL_NORMAL, 0, (int)Control.CursorX) * res.Width));
            int mouseY = Convert.ToInt32(Math.Round(Function.Call<float>(Hash.GET_CONTROL_NORMAL, 0, (int)Control.CursorY) * res.Height));

            return (mouseX >= topLeft.X && mouseX <= topLeft.X + boxSize.Width)
                   && (mouseY > topLeft.Y && mouseY < topLeft.Y + boxSize.Height);
        }


        /// <summary>
        /// Function to get whether the cursor is in an arrow space, or in label of an UIMenuListItem.
        /// </summary>
        /// <param name="item">What item to check</param>
        /// <param name="topLeft">top left point of the item.</param>
        /// <param name="safezone">safezone size.</param>
        /// <returns>0 - Not in item at all, 1 - In label, 2 - In arrow space.</returns>
        public int IsMouseInListItemArrows(UIMenuListItem item, Point topLeft, Point safezone) // TODO: Ability to scroll left and right
        {
            Function.Call((Hash)0x54CE8AC98E120CAB, "jamyfafi");
            UIResText.AddLongString(item.Text);
            var res = GetScreenResolutionMantainRatio();
            var screenw = res.Width;
            var screenh = res.Height;
            const float height = 1080f;
            float ratio = screenw / screenh;
            var width = height * ratio;
            int labelSize = Convert.ToInt32(Function.Call<float>((Hash)0x85F061DA64ED2F67, 0) * width * 0.35f);

            int labelSizeX = 5 + labelSize + 10;
            int arrowSizeX = 431 - labelSizeX;
            return IsMouseInBounds(topLeft, new Size(labelSizeX, 38))
                ? 1
                : IsMouseInBounds(new Point(topLeft.X + labelSizeX, topLeft.Y), new Size(arrowSizeX, 38)) ? 2 : 0;

        }


        /// <summary>
        /// Returns the safezone bounds in pixel, relative to the 1080pixel based system.
        /// </summary>
        /// <returns></returns>
        public static Point GetSafezoneBounds()
        {
            float t = Function.Call<float>(Hash.GET_SAFE_ZONE_SIZE); // Safezone size.
            double g = Math.Round(Convert.ToDouble(t), 2);
            g = (g * 100) - 90;
            g = 10 - g;

            const float hmp = 5.4f;
            int screenw = Screen.Resolution.Width;
            int screenh = Screen.Resolution.Height;
            float ratio = (float)screenw / screenh;
            float wmp = ratio*hmp;
            
            return new Point(Convert.ToInt32(Math.Round(g*wmp)), Convert.ToInt32(Math.Round(g*hmp)));
        }


        /// <summary>
        /// Go up the menu if the number of items is more than maximum items on screen.
        /// </summary>
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
            Audio.PlaySoundFrontend(AUDIO_UPDOWN, AUDIO_LIBRARY);
            IndexChange(CurrentSelection);
        }


        /// <summary>
        /// Go up the menu if the number of items is less than or equal to the maximum items on screen.
        /// </summary>
        public void GoUp()
        {
            if (Size > MaxItemsOnScreen + 1) return;
            MenuItems[_activeItem % (MenuItems.Count)].Selected = false;
            _activeItem--;
            MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
            Audio.PlaySoundFrontend(AUDIO_UPDOWN, AUDIO_LIBRARY);
            IndexChange(CurrentSelection);
        }


        /// <summary>
        /// Go down the menu if the number of items is more than maximum items on screen.
        /// </summary>
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
            Audio.PlaySoundFrontend(AUDIO_UPDOWN, AUDIO_LIBRARY);
            IndexChange(CurrentSelection);
        }


        /// <summary>
        /// Go up the menu if the number of items is less than or equal to the maximum items on screen.
        /// </summary>
        public void GoDown()
        {
            if (Size > MaxItemsOnScreen + 1) return;
            MenuItems[_activeItem % (MenuItems.Count)].Selected = false;
            _activeItem++;
            MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
            Audio.PlaySoundFrontend(AUDIO_UPDOWN, AUDIO_LIBRARY);
            IndexChange(CurrentSelection);
        }


        /// <summary>
        /// Go left on a UIMenuListItem.
        /// </summary>
        public void GoLeft()
        {
            if (!(MenuItems[CurrentSelection] is UIMenuListItem)) return;
            var it = (UIMenuListItem)MenuItems[CurrentSelection];
            it.Index--;
            Audio.PlaySoundFrontend(AUDIO_LEFTRIGHT, AUDIO_LIBRARY);
            ListChange(it, it.Index);
            it.ListChangedTrigger(it.Index);
        }


        /// <summary>
        /// Go right on a UIMenuListItem.
        /// </summary>
        public void GoRight()
        {
            if (!(MenuItems[CurrentSelection] is UIMenuListItem)) return;
            var it = (UIMenuListItem)MenuItems[CurrentSelection];
            it.Index++;
            Audio.PlaySoundFrontend(AUDIO_LEFTRIGHT, AUDIO_LIBRARY);
            ListChange(it, it.Index);
            it.ListChangedTrigger(it.Index);
        }


        /// <summary>
        /// Activate the current selected item.
        /// </summary>
        public void SelectItem()
        {
            if (!MenuItems[CurrentSelection].Enabled)
            {
                Audio.PlaySoundFrontend(AUDIO_ERROR, AUDIO_LIBRARY);
                return;
            }

            Audio.PlaySoundFrontend(AUDIO_SELECT, AUDIO_LIBRARY);
            MenuItems[CurrentSelection].ProcessControl(MenuControls.Select);
        }


        /// <summary>
        /// Close or go back in a menu chain.
        /// </summary>
        public void GoBack()
        {
            Audio.PlaySoundFrontend(AUDIO_BACK, AUDIO_LIBRARY);
            Visible = false;
            if (ParentMenu != null)
            {
                var tmp = Cursor.Position;
                ParentMenu.Visible = true;
                MenuChangeEv(ParentMenu, false);
                if(ResetCursorOnOpen)
                    Cursor.Position = tmp;
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
        /// Process the mouse's position and check if it's hovering over any UI element. Call this in OnTick
        /// </summary>
        public void ProcessMouse()
        {
            if (!Visible || _justOpened || MenuItems.Count == 0 || IsUsingController || !MouseControlsEnabled)
            {
                Function.Call(Hash.ENABLE_CONTROL_ACTION, (int)Control.LookUpDown);
                Function.Call(Hash.ENABLE_CONTROL_ACTION, (int)Control.LookLeftRight);
                Function.Call(Hash.ENABLE_CONTROL_ACTION, (int)Control.Aim);
                Function.Call(Hash.ENABLE_CONTROL_ACTION, (int)Control.Attack);
                MenuItems.Where(i => i.Hovered).ToList().ForEach(i => i.Hovered = false);
                return;
            }

            Point safezoneOffset = GetSafezoneBounds();
            Function.Call(Hash._SHOW_CURSOR_THIS_FRAME);
            int limit = MenuItems.Count - 1;
            int counter = 0;
            if (MenuItems.Count > MaxItemsOnScreen + 1)
                limit = _maxItem;

            if (IsMouseInBounds(new Point(0, 0), new Size(30, 1080)) && MouseEdgeEnabled)
            {
                GameplayCamera.RelativeHeading += 5f;
                Function.Call((Hash) 0x8DB8CFFD58B62552, 6);
            }
            else if (IsMouseInBounds(new Point(Convert.ToInt32(GetScreenResolutionMantainRatio().Width - 30f), 0), new Size(30, 1080)) &&  MouseEdgeEnabled)
            {
                GameplayCamera.RelativeHeading -= 5f;
                Function.Call((Hash) 0x8DB8CFFD58B62552, 7);
            }
            else if(MouseEdgeEnabled)
            {
                Function.Call((Hash) 0x8DB8CFFD58B62552, 1);
            }

            for (int i = _minItem; i <= limit; i++)
            {
                int xpos = _offset.X + safezoneOffset.X;
                int ypos = _offset.Y + 144 - 37 + _extraYOffset + (counter*38) + safezoneOffset.Y;
                int xsize = 431 + WidthOffset;
                const int ysize = 38;
                UIMenuItem uiMenuItem = MenuItems[i];
                if (IsMouseInBounds(new Point(xpos, ypos), new Size(xsize, ysize)))
                {
                    uiMenuItem.Hovered = true;
                    if (Game.IsControlJustPressed(Control.Attack))
                        if (uiMenuItem.Selected && uiMenuItem.Enabled)
                        {
                            if (MenuItems[i] is UIMenuListItem &&
                                IsMouseInListItemArrows((UIMenuListItem) MenuItems[i], new Point(xpos, ypos),
                                    safezoneOffset) > 0)
                            {
                                int res = IsMouseInListItemArrows((UIMenuListItem) MenuItems[i], new Point(xpos, ypos),
                                    safezoneOffset);
                                switch (res)
                                {
                                    case 1:
                                        Audio.PlaySoundFrontend(AUDIO_SELECT, AUDIO_LIBRARY);
                                        MenuItems[i].ItemActivate(this);
                                        ItemSelect(MenuItems[i], i);
                                        break;
                                    case 2:
                                        var it = (UIMenuListItem) MenuItems[i];
                                        it.Index++;
                                        Audio.PlaySoundFrontend(AUDIO_LEFTRIGHT, AUDIO_LIBRARY);
                                        ListChange(it, it.Index);
                                        it.ListChangedTrigger(it.Index);
                                        break;
                                }
                            }
                            else
                                SelectItem();
                        }
                        else if(!uiMenuItem.Selected)
                        {
                            CurrentSelection = i;
                            Audio.PlaySoundFrontend(AUDIO_UPDOWN, AUDIO_LIBRARY);
                            IndexChange(CurrentSelection);
                            UpdateScaleform();
                        }
                        else if (!uiMenuItem.Enabled && uiMenuItem.Selected)
                        {
                            Audio.PlaySoundFrontend(AUDIO_ERROR, AUDIO_LIBRARY);
                        }
                }
                else
                    uiMenuItem.Hovered = false;
                counter++;
            }
            int extraY = 144 + 38*(MaxItemsOnScreen + 1) + _offset.Y - 37 + _extraYOffset + safezoneOffset.Y;
            int extraX = safezoneOffset.X + _offset.X;
            if(Size <= MaxItemsOnScreen + 1) return;
            if (IsMouseInBounds(new Point(extraX, extraY), new Size(431 + WidthOffset, 18)))
            {
                _extraRectangleUp.Color = Color.FromArgb(255, 30, 30, 30);
                if (Game.IsControlJustPressed(Control.Attack))
                {
                    if(Size > MaxItemsOnScreen+1)
                        GoUpOverflow();
                    else
                        GoUp();
                }
            }
            else
                _extraRectangleUp.Color = Color.FromArgb(200, 0, 0, 0);
            
            if (IsMouseInBounds(new Point(extraX, extraY+18), new Size(431 + WidthOffset, 18)))
            {
                _extraRectangleDown.Color = Color.FromArgb(255, 30, 30, 30);
                if (Game.IsControlJustPressed(Control.Attack))
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
        public void SetKey(MenuControls control, Control gtaControl)
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
        public void SetKey(MenuControls control, Control gtaControl, int controlIndex)
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


        /// <summary>
        /// Check whether a menucontrol has been pressed.
        /// </summary>
        /// <param name="control">Control to check for.</param>
        /// <param name="key">Key if you're using keys.</param>
        /// <returns></returns>
        public bool HasControlJustBeenPressed(MenuControls control, Keys key = Keys.None)
        {
            List<Keys> tmpKeys = new List<Keys>(_keyDictionary[control].Item1);
            List<Tuple<Control, int>> tmpControls = new List<Tuple<Control, int>>(_keyDictionary[control].Item2);

            if (key != Keys.None)
            {
                if (tmpKeys.Any(Game.IsKeyPressed))
                    return true;
            }
            if (tmpControls.Any(tuple => Game.IsControlJustPressed(tuple.Item1)))
                return true;
            return false;
        }


        /// <summary>
        /// Check whether a menucontrol has been released.
        /// </summary>
        /// <param name="control">Control to check for.</param>
        /// <param name="key">Key if you're using keys.</param>
        /// <returns></returns>
        public bool HasControlJustBeenReleased(MenuControls control, Keys key = Keys.None)
        {
            List<Keys> tmpKeys = new List<Keys>(_keyDictionary[control].Item1);
            List<Tuple<Control, int>> tmpControls = new List<Tuple<Control, int>>(_keyDictionary[control].Item2);

            if (key != Keys.None)
            {
                if (tmpKeys.Any(Game.IsKeyPressed))
                    return true;
            }
            if (tmpControls.Any(tuple => Game.IsControlJustReleased(tuple.Item1)))
                return true;
            return false;
        }

        private int _controlCounter;
        /// <summary>
        /// Check whether a menucontrol is being pressed.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsControlBeingPressed(MenuControls control, Keys key = Keys.None)
        {
            List<Keys> tmpKeys = new List<Keys>(_keyDictionary[control].Item1);
            List<Tuple<Control, int>> tmpControls = new List<Tuple<Control, int>>(_keyDictionary[control].Item2);
            if (HasControlJustBeenReleased(control, key)) _controlCounter = 0;
            if (_controlCounter > 0)
            {
                _controlCounter++;
                if (_controlCounter > 30)
                    _controlCounter = 0;
                return false;
            }
            if (key != Keys.None)
            {
                if (tmpKeys.Any(Game.IsKeyPressed))
                {
                    _controlCounter = 1;
                    return true;
                }
            }
            if (tmpControls.Any(tuple => Game.IsControlPressed(tuple.Item1)))
            {
                _controlCounter = 1;
                return true;
            }
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

            if (HasControlJustBeenReleased(MenuControls.Back, key))
            {
                if (MenuItems.Count == 0 || MenuItems[CurrentSelection].ProcessControl(MenuControls.Back))
                    GoBack();
            }

            if (MenuItems.Count == 0) return;

            if (IsControlBeingPressed(MenuControls.Up, key) && MenuItems[CurrentSelection].ProcessControl(MenuControls.Up))
            {
                if (Size > MaxItemsOnScreen + 1)
                    GoUpOverflow();
                else
                    GoUp();
                UpdateScaleform();
            }

            else if (IsControlBeingPressed(MenuControls.Down, key) && MenuItems[CurrentSelection].ProcessControl(MenuControls.Down))
            {
                if (Size > MaxItemsOnScreen + 1)
                    GoDownOverflow();
                else
                    GoDown();
                UpdateScaleform();
            }

            else if (IsControlBeingPressed(MenuControls.Left, key))
            {
                MenuItems[CurrentSelection].ProcessControl(MenuControls.Left);
            }

            else if (IsControlBeingPressed(MenuControls.Right, key))
            {
                MenuItems[CurrentSelection].ProcessControl(MenuControls.Right);
            }

            else if (HasControlJustBeenPressed(MenuControls.Select, key))
            {
                SelectItem();
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


        /// <summary>
        /// Formats the input string so it doesn't go out of bounds of the description box.
        /// </summary>
        /// <param name="input">String to format.</param>
        /// <returns></returns>
        private string FormatDescription(string input)
        {
            int maxPixelsPerLine = 425 + WidthOffset;
            int aggregatePixels = 0;
            string output = "";
            string[] words = input.Split(' ');
            foreach (string word in words)
            {
                int offset = StringMeasurer.MeasureString(word);
                aggregatePixels += offset;
                if (aggregatePixels > maxPixelsPerLine)
                {
                    output += "\n" + word + " ";
                    aggregatePixels = offset + StringMeasurer.MeasureString(" ");
                }
                else
                {
                    output += word + " ";
                    aggregatePixels += StringMeasurer.MeasureString(" ");
                }
            }
            return output;
        }

        private readonly List<InstructionalButton> _instructionalButtons = new List<InstructionalButton>();

        public void AddInstructionalButton(InstructionalButton button)
        {
            _instructionalButtons.Add(button);
        }

        public void RemoveInstructionalButton(InstructionalButton button)
        {
            _instructionalButtons.Remove(button);
        }

        private readonly Scaleform _instructionalButtonsScaleform;
        /// <summary>
        /// Manually update the instructional buttons scaleform.
        /// </summary>
        public void UpdateScaleform()
        {   
            if (!Visible || !_buttonsEnabled) return;
            _instructionalButtonsScaleform.CallFunction("CLEAR_ALL");
            _instructionalButtonsScaleform.CallFunction("TOGGLE_MOUSE_BUTTONS", 0);
            _instructionalButtonsScaleform.CallFunction("CREATE_CONTAINER");
            

            _instructionalButtonsScaleform.CallFunction("SET_DATA_SLOT", 0, Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, (int)Control.PhoneSelect, 0), "Select");
            _instructionalButtonsScaleform.CallFunction("SET_DATA_SLOT", 1, Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 2, (int)Control.PhoneCancel, 0), "Back");
            int count = 2;
            foreach (var button in _instructionalButtons.Where(button => button.ItemBind == null || MenuItems[CurrentSelection] == button.ItemBind))
            {
                _instructionalButtonsScaleform.CallFunction("SET_DATA_SLOT", count, button.GetButtonId(), button.Text);
                count++;
            }
            _instructionalButtonsScaleform.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", -1);
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
                if (ParentMenu != null || !value) return;
                if (!ResetCursorOnOpen) return;
                Cursor.Position = new Point(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width/2, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height/2);
                Function.Call((Hash) 0x8DB8CFFD58B62552, 1);
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
        /// Returns false if last input was made with mouse and keyboard, true if it was made with a controller.
        /// </summary>
        public static bool IsUsingController => !Function.Call<bool>(Hash._IS_INPUT_DISABLED, 2);


        /// <summary>
        /// Returns the amount of items in the menu.
        /// </summary>
        public int Size => MenuItems.Count;


        /// <summary>
        /// Returns the title object.
        /// </summary>
        public UIResText Title { get; }


        /// <summary>
        /// Returns the subtitle object.
        /// </summary>
        public UIResText Subtitle { get; }


        /// <summary>
        /// String to pre-attach to the counter string. Useful for color codes.
        /// </summary>
        public string CounterPretext { get; set; }


        /// <summary>
        /// If this is a nested menu, returns the parent menu. You can also set it to a menu so when pressing Back it goes to that menu.
        /// </summary>
        public UIMenu ParentMenu { get; set; }

        
        /// <summary>
        /// If this is a nested menu, returns the item it was binded to.
        /// </summary>
        public UIMenuItem ParentItem { get; set; }


        internal virtual void IndexChange(int newindex)
        {
            OnIndexChange?.Invoke(this, newindex);
        }


        internal virtual void ListChange(UIMenuListItem sender, int newindex)
        {
            OnListChange?.Invoke(this, sender, newindex);
        }


        internal virtual void ItemSelect(UIMenuItem selecteditem, int index)
        {
            OnItemSelect?.Invoke(this, selecteditem, index);
        }


        internal virtual void CheckboxChange(UIMenuCheckboxItem sender, bool Checked)
        {
            OnCheckboxChange?.Invoke(this, sender, Checked);
        }
        
        internal virtual void MenuCloseEv()
        {
            OnMenuClose?.Invoke(this);
        }

        internal virtual void MenuChangeEv(UIMenu newmenu, bool forward)
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
