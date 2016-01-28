using System;
using System.Collections.Generic;
using System.Drawing;
using GTA;
using GTA.Native;
using Font = GTA.Font;

namespace NativeUI.PauseMenu
{
    public class TabView
    {
        public TabView(string title)
        {
            Title = title;
            Tabs = new List<TabItem>();

            Name = Game.Player.Name;
            IsControlInTabs = true;
        }

        public string Title { get; set; }
        public Sprite Photo { get; set; }
        public string Name { get; set; }
        public string Money { get; set; }
        public string MoneySubtitle { get; set; }
        public List<TabItem> Tabs { get; set; }
        public bool IsControlInTabs { get; set; }

        public event EventHandler OnMenuClose;

        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;

                if (value)
                {
                    Function.Call(Hash._START_SCREEN_EFFECT, "MinigameTransitionIn", 0, true);

                }
                else
                {
                    Function.Call(Hash._STOP_SCREEN_EFFECT, "MinigameTransitionIn");
                }
            }
        }


        public int Index;
        private bool _visible;

        public void ProcessMouse()
        {
        }

        private Scaleform _sc;
        public void ShowInstructionalButtons()
        {
            if (_sc == null)
            {
                _sc = new Scaleform(0);
                _sc.Load("instructional_buttons");
            }

            _sc.CallFunction("CLEAR_ALL");
            _sc.CallFunction("TOGGLE_MOUSE_BUTTONS", 0);
            _sc.CallFunction("CREATE_CONTAINER");


            _sc.CallFunction("SET_DATA_SLOT", 0, Function.Call<string>(Hash._0x0499D7B09FC9B407, 2, (int)Control.PhoneSelect, 0), "Select");
            _sc.CallFunction("SET_DATA_SLOT", 1, Function.Call<string>(Hash._0x0499D7B09FC9B407, 2, (int)Control.PhoneCancel, 0), "Back");

            _sc.CallFunction("SET_DATA_SLOT", 2, Function.Call<string>(Hash._0x0499D7B09FC9B407, 2, (int)Control.FrontendRb, 0), "");
            _sc.CallFunction("SET_DATA_SLOT", 3, Function.Call<string>(Hash._0x0499D7B09FC9B407, 2, (int)Control.FrontendLb, 0), "Browse");

        }

        public void ProcessControls()
        {
            Function.Call(Hash.DISABLE_ALL_CONTROL_ACTIONS, 0);

            if (Game.IsControlJustPressed(0, Control.PhoneLeft) && IsControlInTabs)
            {
                Tabs[Index].Active = false;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = false;
                Index = (1000 - (1000 % Tabs.Count) + Index - 1) % Tabs.Count;
                Tabs[Index].Active = true;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = true;

                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);
            }

            else if (Game.IsControlJustPressed(0, Control.PhoneRight) && IsControlInTabs)
            {
                Tabs[Index].Active = false;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = false;
                Index = (1000 - (1000 % Tabs.Count) + Index + 1) % Tabs.Count;
                Tabs[Index].Active = true;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = true;

                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);
            }

            else if (Game.IsControlJustPressed(0, Control.FrontendAccept) && IsControlInTabs)
            {
                if (Tabs[Index].CanBeFocused)
                {
                    Tabs[Index].Focused = true;
                    Tabs[Index].JustOpened = true;
                    IsControlInTabs = false;
                }
                else
                {
                    Tabs[Index].JustOpened = true;
                    Tabs[Index].OnActivated();
                }

                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);

            }

            else if (Game.IsControlJustPressed(0, Control.PhoneCancel) && !IsControlInTabs)
            {
                Tabs[Index].Focused = false;
                IsControlInTabs = true;

                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "BACK", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);
            }

            else if (Game.IsControlJustPressed(0, Control.PhoneCancel) && IsControlInTabs)
            {
                Visible = false;
                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "BACK", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);

                OnMenuClose?.Invoke(this, EventArgs.Empty);
            }

            else if (Game.IsControlJustPressed(0, Control.FrontendLb))
            {
                Tabs[Index].Active = false;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = false;
                Index = (1000 - (1000 % Tabs.Count) + Index - 1) % Tabs.Count;
                Tabs[Index].Active = true;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = true;

                IsControlInTabs = true;

                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);
            }

            else if (Game.IsControlJustPressed(0, Control.FrontendRb))
            {
                Tabs[Index].Active = false;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = false;
                Index = (1000 - (1000 % Tabs.Count) + Index + 1) % Tabs.Count;
                Tabs[Index].Active = true;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = true;

                IsControlInTabs = true;

                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);
            }

        }

        public void RefreshIndex()
        {
            Index = (1000 - (1000 % Tabs.Count)) % Tabs.Count;
            Tabs[Index].Active = true;
            Tabs[Index].Focused = false;
            Tabs[Index].Visible = true;
        }

        public void Update()
        {
            if (!Visible) return;

            ShowInstructionalButtons();
            Function.Call(Hash.HIDE_HUD_AND_RADAR_THIS_FRAME);

            ProcessControls();
            ProcessMouse();

            var res = UIMenu.GetScreenResolutionMantainRatio();
            var safe = new Point(300, 180);

            new UIResText(Title, new Point(safe.X, safe.Y - 80), 1f, Color.White, Font.ChaletComprimeCologne, UIResText.Alignment.Left)
            {
                DropShadow = true,
            }.Draw();

            if (Photo == null)
            {
                new Sprite("char_multiplayer", "char_multiplayer", new Point((int)res.Width - safe.X - 64, safe.Y - 80), new Size(64, 64)).Draw();
            }
            else
            {
                Photo.Position = new Point((int)res.Width - safe.X - 100, safe.Y - 80);
                Photo.Size = new Size(64, 64);
                Photo.Draw();
            }

            new UIResText(Name, new Point((int)res.Width - safe.X - 70, safe.Y - 95), 0.7f, Color.White,
                Font.ChaletComprimeCologne, UIResText.Alignment.Right)
            {
                DropShadow = true,
            }.Draw();

            string subt = Money;
            if (string.IsNullOrEmpty(Money))
            {
                subt = DateTime.Now.ToString();
            }


            new UIResText(subt, new Point((int)res.Width - safe.X - 70, safe.Y - 60), 0.4f, Color.White,
                Font.ChaletComprimeCologne, UIResText.Alignment.Right)
            {
                DropShadow = true,
            }.Draw();

            new UIResText(MoneySubtitle, new Point((int)res.Width - safe.X - 70, safe.Y - 40), 0.4f, Color.White,
                Font.ChaletComprimeCologne, UIResText.Alignment.Right)
            {
                DropShadow = true,
            }.Draw();

            for (int i = 0; i < Tabs.Count; i++)
            {

                var activeSize = res.Width - 2 * safe.X;
                activeSize -= 4 * 5;
                int tabWidth = (int)activeSize / 5;

                var tabColor = Tabs[i].Active ? Color.White : Color.Black;
                new UIResRectangle(safe.AddPoints(new Point((tabWidth + 5) * i, 0)), new Size(tabWidth, 40), Color.FromArgb(Tabs[i].Active ? 255 : 200, tabColor)).Draw();

                new UIResText(Tabs[i].Title.ToUpper(), safe.AddPoints(new Point((tabWidth / 2) + (tabWidth + 5) * i, 5)), 0.35f,
                    Tabs[i].Active ? Color.Black : Color.White, Font.ChaletLondon, UIResText.Alignment.Centered).Draw();

                if (Tabs[i].Active)
                {
                    new UIResRectangle(safe.SubtractPoints(new Point(-((tabWidth + 5) * i), 10)), new Size(tabWidth, 10), Color.DodgerBlue).Draw();
                }
            }

            Tabs[Index].Draw();

            _sc.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", -1);

            _sc.Render2D();
        }
    }

}