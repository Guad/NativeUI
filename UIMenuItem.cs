using System.Drawing;
using GTA;

namespace NativeUI
{
    public class UIMenuItem
    {
        private readonly UIResRectangle _rectangle;
        private readonly UIResText _text;
        private bool _selected;
        private Sprite _selectedSprite;

        private Sprite _badgeLeft;
        private Sprite _badgeRight;

        /// <summary>
        /// Basic menu button.
        /// </summary>
        /// <param name="text">Button label.</param>
        public UIMenuItem(string text) : this(text, "")
        {
        }

        /// <summary>
        /// Basic menu button.
        /// </summary>
        /// <param name="text">Button label.</param>
        /// <param name="description">Description.</param>
        public UIMenuItem(string text, string description)
        {
            Text = text;
            _rectangle = new UIResRectangle(new Point(0, 0), new Size(431, 38), Color.FromArgb(150, 0, 0, 0));
            _text = new UIResText(text, new Point(8, 0), 0.33f, Color.WhiteSmoke, GTA.Font.ChaletLondon, false);
            Description = description;
            _selectedSprite = new Sprite("commonmenu", "gradient_nav", new Point(0, 0), new Size(431, 38));

            _badgeLeft = new Sprite("commonmenu", "", new Point(0, 0), new Size(40, 40));
            _badgeRight = new Sprite("commonmenu", "", new Point(0, 0), new Size(40, 40));
        }


        /// <summary>
        /// Whether this item is currently selected.
        /// </summary>
        public virtual bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                
                _text.Color = value ? Color.Black : Color.WhiteSmoke;
            }
        }

        public virtual bool Hovered { get; set; }

        public virtual string Description { get; set; }
        
        /// <summary>
        /// Set item's position.
        /// </summary>
        /// <param name="y"></param>
        public virtual void Position(int y)
        {
            _rectangle.Position = new Point(Offset.X, y + 144 + Offset.Y);
            _selectedSprite.Position = new Point(0 + Offset.X, y + 144 + Offset.Y);
            _text.Position = new Point(8 + Offset.X, y + 147 + Offset.Y);

            _badgeLeft.Position = new Point(0 + Offset.X, y + 142 + Offset.Y);
            _badgeRight.Position = new Point(385 + Offset.X, y + 142 + Offset.Y);
        }


        /// <summary>
        /// Draw this item.
        /// </summary>
        public virtual void Draw()
        {
            if (Hovered && !Selected)
            {
                _rectangle.Color = Color.FromArgb(20, 255, 255, 255);
                _rectangle.Draw();
            }
            if (Selected)
                _selectedSprite.Draw();
            if (LeftBadge != BadgeStyle.None)
            {
                _text.Position = new Point(35 + Offset.X, _text.Position.Y);
                _badgeLeft.TextureName = BadgeToSprite(LeftBadge, Selected);
                _badgeLeft.Color = BadgeToColor(LeftBadge, Selected);
                _badgeLeft.Draw();
            }
            else
            {
                _text.Position = new Point(8 + Offset.X, _text.Position.Y);
            }
            if (RightBadge != BadgeStyle.None)
            {
                _badgeRight.TextureName = BadgeToSprite(RightBadge, Selected);
                _badgeRight.Color = BadgeToColor(RightBadge, Selected);
                _badgeRight.Draw();
            }
            _text.Draw();
        }

        public Point Offset { get; set; }

        public string Text { get; set; }

        public virtual BadgeStyle LeftBadge { get; set; }

        public virtual BadgeStyle RightBadge { get; set; }

        public enum BadgeStyle
        {
            None,
            BronzeMedal,
            GoldMedal,
            SilverMedal,
            Alert,
            Crown,
            Ammo,
            Armour,
            Barber,
            Clothes,
            Franklin,
            Bike,
            Car,
            Gun,
            Heart,
            Makeup,
            Mask,
            Michael,
            Star,
            Tatoo,
            Trevor,
            Lock,
            Tick,
        }

        private string BadgeToSprite(BadgeStyle badge, bool selected)
        {
            switch (badge)
            {
                case BadgeStyle.None:
                    return "";
                case BadgeStyle.BronzeMedal:
                    return "mp_medal_bronze";
                case BadgeStyle.GoldMedal:
                    return "mp_medal_gold";
                case BadgeStyle.SilverMedal:
                    return "medal_silver";
                case BadgeStyle.Alert:
                    return "mp_alerttriangle";
                case BadgeStyle.Crown:
                    return "mp_hostcrown";
                case BadgeStyle.Ammo:
                    return selected ? "shop_ammo_icon_b" : "shop_ammo_icon_a";
                case BadgeStyle.Armour:
                    return selected ? "shop_armour_icon_b" : "shop_armour_icon_a";
                case BadgeStyle.Barber:
                    return selected ? "shop_barber_icon_b" : "shop_barber_icon_a";
                case BadgeStyle.Clothes:
                    return selected ? "shop_clothing_icon_b" : "shop_clothing_icon_a";
                case BadgeStyle.Franklin:
                    return selected ? "shop_franklin_icon_b" : "shop_franklin_icon_a";
                case BadgeStyle.Bike:
                    return selected ? "shop_garage_bike_icon_b" : "shop_garage_bike_icon_a";
                case BadgeStyle.Car:
                    return selected ? "shop_garage_icon_b" : "shop_garage_icon_a";
                case BadgeStyle.Gun:
                    return selected ? "shop_gunclub_icon_b" : "shop_gunclub_icon_a";
                case BadgeStyle.Heart:
                    return selected ? "shop_health_icon_b" : "shop_health_icon_a";
                case BadgeStyle.Lock:
                    return "shop_lock";
                case BadgeStyle.Makeup:
                    return selected ? "shop_makeup_icon_b" : "shop_makeup_icon_a";
                case BadgeStyle.Mask:
                    return selected ? "shop_mask_icon_b" : "shop_mask_icon_a";
                case BadgeStyle.Michael:
                    return selected ? "shop_michael_icon_b" : "shop_michael_icon_a";
                case BadgeStyle.Star:
                    return "shop_new_star";
                case BadgeStyle.Tatoo:
                    return selected ? "shop_tattoos_icon_b" : "shop_tattoos_icon_";
                case BadgeStyle.Tick:
                    return "shop_tick_icon";
                case BadgeStyle.Trevor:
                    return selected ? "shop_trevor_icon_b" : "shop_trevor_icon_a";
                default:
                    return "";
            }
        }

        public Color BadgeToColor(BadgeStyle badge, bool selected)
        {
            switch (badge)
            {
                case BadgeStyle.Lock:
                case BadgeStyle.Tick:
                case BadgeStyle.Crown:
                    return selected ? Color.FromArgb(255, 0, 0, 0) : Color.FromArgb(255, 255, 255, 255);
                default:
                    return Color.FromArgb(255, 255, 255, 255);
            }
        }

        public UIMenu Parent { get; set; }
    }
}
