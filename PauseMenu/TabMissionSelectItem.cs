using System;
using System.Collections.Generic;
using System.Drawing;
using GTA;
using GTA.Native;
using Font = GTA.Font;

namespace NativeUI.PauseMenu
{
    public delegate void OnItemSelect(MissionInformation selectedItem);

    public class MissionInformation
    {
        public MissionInformation(string name, IEnumerable<Tuple<string, string>> info)
        {
            Name = name;
            ValueList = new List<Tuple<string, string>>(info);
        }

        public MissionInformation(string name, string description, IEnumerable<Tuple<string, string>> info)
        {
            Name = name;
            Description = description;
            ValueList = new List<Tuple<string, string>>(info);
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public MissionLogo Logo { get; set; }
        public List<Tuple<string, string>> ValueList { get; set; }
    }

    public class MissionLogo
    {
        /// <summary>
        /// Create a logo from an external picture.
        /// </summary>
        /// <param name="filepath">Path to the picture</param>
        public MissionLogo(string filepath)
        {
            FileName = filepath;
            IsGameTexture = false;
        }

        /// <summary>
        /// Create a mission logo from a game texture.
        /// </summary>
        /// <param name="textureDict">Name of the texture dictionary</param>
        /// <param name="textureName">Name of the texture.</param>
        public MissionLogo(string textureDict, string textureName)
        {
            FileName = textureName;
            DictionaryName = textureDict;
            IsGameTexture = true;
        }

        internal bool IsGameTexture;
        internal string FileName { get; set; }
        internal string DictionaryName { get; set; }
    }

    public class TabMissionSelectItem : TabItem
    {
        public TabMissionSelectItem(string name, IEnumerable<MissionInformation> list) : base(name)
        {
            base.FadeInWhenFocused = true;
            base.DrawBg = false;

            _noLogo = new Sprite("gtav_online", "rockstarlogo256", new Point(), new Size(512, 256));
            _maxItem = MaxItemsPerView;
            _minItem = 0;

            CanBeFocused = true;

            Heists = new List<MissionInformation>(list);
        }

        public event OnItemSelect OnItemSelect;

        public List<MissionInformation> Heists { get; set; }
        public int Index { get; set; }

        protected const int MaxItemsPerView = 15;
        protected int _minItem;
        protected int _maxItem;
        protected Sprite _noLogo { get; set; }

        public override void ProcessControls()
        {
            if (!Focused) return;
            if (Heists.Count == 0) return;
            if (JustOpened)
            {
                JustOpened = false;
                return;
            }

            if (Game.IsControlJustPressed(0, Control.PhoneSelect))
            {
                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);
                OnItemSelect?.Invoke(Heists[Index]);
            }

            if (Game.IsControlJustPressed(0, Control.FrontendUp) || Game.IsControlJustPressed(0, Control.MoveUpOnly))
            {
                Index = (1000 - (1000 % Heists.Count) + Index - 1) % Heists.Count;
                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);

                if (Heists.Count <= MaxItemsPerView) return;

                if (Index < _minItem)
                {
                    _minItem--;
                    _maxItem--;
                }

                if (Index == Heists.Count - 1)
                {
                    _minItem = Heists.Count - MaxItemsPerView;
                    _maxItem = Heists.Count;
                }
            }

            else if (Game.IsControlJustPressed(0, Control.FrontendDown) || Game.IsControlJustPressed(0, Control.MoveDownOnly))
            {
                Index = (1000 - (1000 % Heists.Count) + Index + 1) % Heists.Count;
                Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);

                if (Heists.Count <= MaxItemsPerView) return;

                if (Index >= _maxItem)
                {
                    _maxItem++;
                    _minItem++;
                }

                if (Index == 0)
                {
                    _minItem = 0;
                    _maxItem = MaxItemsPerView;
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (Heists.Count == 0) return;

            var res = UIMenu.GetScreenResolutionMaintainRatio();

            var activeWidth = res.Width - SafeSize.X * 2;
            var itemSize = new Size((int)activeWidth - 515, 40);

            var alpha = Focused ? 120 : 30;
            var blackAlpha = Focused ? 200 : 100;
            var fullAlpha = Focused ? 255 : 150;

            var counter = 0;
            for (int i = _minItem; i < Math.Min(Heists.Count, _maxItem); i++)
            {
                new UIResRectangle(SafeSize.AddPoints(new Point(0, (itemSize.Height + 3) * counter)), itemSize, (Index == i && Focused) ? Color.FromArgb(fullAlpha, Color.White) : Color.FromArgb(blackAlpha, Color.Black)).Draw();
                new UIResText(Heists[i].Name, SafeSize.AddPoints(new Point(6, 5 + (itemSize.Height + 3) * counter)), 0.35f, Color.FromArgb(fullAlpha, (Index == i && Focused) ? Color.Black : Color.White)).Draw();
                counter++;
            }

            if (Heists[Index].Logo == null || string.IsNullOrEmpty(Heists[Index].Logo.FileName))
            {
                _noLogo.Position = new Point((int)res.Width - SafeSize.X - 512, SafeSize.Y);
                _noLogo.Color = Color.FromArgb(blackAlpha, 0, 0, 0);
                _noLogo.Draw();
            }
            else if (Heists[Index].Logo != null && Heists[Index].Logo.FileName != null && !Heists[Index].Logo.IsGameTexture)
            {
                var target = Heists[Index].Logo.FileName;
                Sprite.DrawTexture(target, new Point((int)res.Width - SafeSize.X - 512, SafeSize.Y), new Size(512, 256));
            }
            else if (Heists[Index].Logo != null && Heists[Index].Logo.FileName != null &&
                     Heists[Index].Logo.IsGameTexture)
            {
                var newLogo = new Sprite(Heists[Index].Logo.DictionaryName, Heists[Index].Logo.FileName, new Point(), new Size(512, 256));
                newLogo.Position = new Point((int)res.Width - SafeSize.X - 512, SafeSize.Y);
                newLogo.Color = Color.FromArgb(blackAlpha, 0, 0, 0);
                newLogo.Draw();
            }

            new UIResRectangle(new Point((int)res.Width - SafeSize.X - 512, SafeSize.Y + 256), new Size(512, 40), Color.FromArgb(fullAlpha, Color.Black)).Draw();
            new UIResText(Heists[Index].Name, new Point((int)res.Width - SafeSize.X - 4, SafeSize.Y + 260), 0.5f, Color.FromArgb(fullAlpha, Color.White),
                Font.HouseScript, UIResText.Alignment.Right).Draw();

            for (int i = 0; i < Heists[Index].ValueList.Count; i++)
            {
                new UIResRectangle(new Point((int)res.Width - SafeSize.X - 512, SafeSize.Y + 256 + 40 + (40 * i)),
                    new Size(512, 40), i % 2 == 0 ? Color.FromArgb(alpha, 0, 0, 0) : Color.FromArgb(blackAlpha, 0, 0, 0)).Draw();
                var text = Heists[Index].ValueList[i].Item1;
                var label = Heists[Index].ValueList[i].Item2;


                new UIResText(text, new Point((int)res.Width - SafeSize.X - 506, SafeSize.Y + 260 + 42 + (40 * i)), 0.35f, Color.FromArgb(fullAlpha, Color.White)).Draw();
                new UIResText(label, new Point((int)res.Width - SafeSize.X - 6, SafeSize.Y + 260 + 42 + (40 * i)), 0.35f, Color.FromArgb(fullAlpha, Color.White), Font.ChaletLondon, UIResText.Alignment.Right).Draw();
            }

            if (!string.IsNullOrEmpty(Heists[Index].Description))
            {
                var propLen = Heists[Index].ValueList.Count;
                new UIResRectangle(new Point((int)res.Width - SafeSize.X - 512, SafeSize.Y + 256 + 42 + 40 * propLen),
                    new Size(512, 2), Color.FromArgb(fullAlpha, Color.White)).Draw();
                new UIResText(Heists[Index].Description,
                    new Point((int)res.Width - SafeSize.X - 508, SafeSize.Y + 256 + 45 + 40 * propLen + 4), 0.35f,
                    Color.FromArgb(fullAlpha, Color.White))
                {
                    WordWrap = new Size(508, 0),
                }.Draw();

                new UIResRectangle(new Point((int) res.Width - SafeSize.X - 512, SafeSize.Y + 256 + 44 + 40*propLen),
                    new Size(512, 45*(int)(StringMeasurer.MeasureString(Heists[Index].Description, (Font) 0, 0.35f)/500)),
                    Color.FromArgb(blackAlpha, 0, 0, 0)).Draw();
            }
        }
    }
}