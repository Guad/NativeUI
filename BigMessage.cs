using System;
using System.Runtime.Remoting.Messaging;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;

namespace NativeUI
{
    public class BigMessageHandler
    {
        private Scaleform _sc;
        private int _start;
        private int _timer;

        public BigMessageHandler()
        {

        }

        public async Task Load()
        {
            if (_sc != null) return;
            _sc = new Scaleform("MP_BIG_MESSAGE_FREEMODE");
            var timeout = 1000;
            var start = DateTime.Now;
            while (!Function.Call<bool>(Hash.HAS_SCALEFORM_MOVIE_LOADED, _sc.Handle) && DateTime.Now.Subtract(start).TotalMilliseconds < timeout) await BaseScript.Delay(0);
        }

        public void Dispose()
        {
            Function.Call(Hash.SET_SCALEFORM_MOVIE_AS_NO_LONGER_NEEDED, new OutputArgument(_sc.Handle));
            _sc = null;
        }

        public async void ShowMissionPassedMessage(string msg, int time = 5000)
        {
            await Load();
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_MISSION_PASSED_MESSAGE", msg, "", 100, true, 0, true);
            _timer = time;
        }

        public async void ShowColoredShard(string msg, string desc, HudColor textColor, HudColor bgColor, int time = 5000)
        {
            await Load();
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_SHARD_CENTERED_MP_MESSAGE", msg, desc, (int)bgColor, (int)textColor);
            _timer = time;
        }

        public async void ShowOldMessage(string msg, int time = 5000)
        {
            await Load();
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_MISSION_PASSED_MESSAGE", msg);
            _timer = time;
        }

        public async void ShowSimpleShard(string title, string subtitle, int time = 5000)
        {
            await Load();
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_SHARD_CREW_RANKUP_MP_MESSAGE", title, subtitle);
            _timer = time;
        }

        public async void ShowRankupMessage(string msg, string subtitle, int rank, int time = 5000)
        {
            await Load();
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_BIG_MP_MESSAGE", msg, subtitle, rank, "", "");
            _timer = time;
        }

        public async void ShowWeaponPurchasedMessage(string bigMessage, string weaponName, WeaponHash weapon, int time = 5000)
        {
            await Load();
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_WEAPON_PURCHASED", bigMessage, weaponName, unchecked((int)weapon), "", 100);
            _timer = time;
        }

        public async void ShowMpMessageLarge(string msg, int time = 5000)
        {
            await Load();
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_CENTERED_MP_MESSAGE_LARGE", msg, "test", 100, true, 100);
            _sc.CallFunction("TRANSITION_IN");
            _timer = time;
        }

        public async void ShowCustomShard(string funcName, params object[] paremeters)
        {
            await Load();
            _sc.CallFunction(funcName, paremeters);
        }

        internal void Update()
        {
            if (_sc == null) return;
            _sc.Render2D();
            if (_start != 0 && Game.GameTime - _start > _timer)
            {
                _sc.CallFunction("TRANSITION_OUT");
                _start = 0;
                Dispose();
            }

        }
    }

    public class BigMessageThread : BaseScript
    {
        public static BigMessageHandler MessageInstance { get; set; }

        public BigMessageThread()
        {
            MessageInstance = new BigMessageHandler();
            Tick += BigMessageThread_Tick;
        }

        private async Task BigMessageThread_Tick()
        {
            MessageInstance.Update();
        }
    }

    public enum HudColor
    {
        HUD_COLOUR_PURE_WHITE = 0,
        HUD_COLOUR_WHITE = 1,
        HUD_COLOUR_BLACK = 2,
        HUD_COLOUR_GREY = 3,
        HUD_COLOUR_GREYLIGHT = 4,
        HUD_COLOUR_GREYDARK = 5,
        HUD_COLOUR_RED = 6,
        HUD_COLOUR_REDLIGHT = 7,
        HUD_COLOUR_REDDARK = 8,
        HUD_COLOUR_BLUE = 9,
        HUD_COLOUR_BLUELIGHT = 10,
        HUD_COLOUR_BLUEDARK = 11,
        HUD_COLOUR_YELLOW = 12,
        HUD_COLOUR_YELLOWLIGHT = 13,
        HUD_COLOUR_YELLOWDARK = 14,
        HUD_COLOUR_ORANGE = 15,
        HUD_COLOUR_ORANGELIGHT = 16,
        HUD_COLOUR_ORANGEDARK = 17,
        HUD_COLOUR_GREEN = 18,
        HUD_COLOUR_GREENLIGHT = 19,
        HUD_COLOUR_GREENDARK = 20,
        HUD_COLOUR_PURPLE = 21,
        HUD_COLOUR_PURPLELIGHT = 22,
        HUD_COLOUR_PURPLEDARK = 23,
        HUD_COLOUR_PINK = 24,
        HUD_COLOUR_RADAR_HEALTH = 25,
        HUD_COLOUR_RADAR_ARMOUR = 26,
        HUD_COLOUR_RADAR_DAMAGE = 27,
        HUD_COLOUR_NET_PLAYER1 = 28,
        HUD_COLOUR_NET_PLAYER2 = 29,
        HUD_COLOUR_NET_PLAYER3 = 30,
        HUD_COLOUR_NET_PLAYER4 = 31,
        HUD_COLOUR_NET_PLAYER5 = 32,
        HUD_COLOUR_NET_PLAYER6 = 33,
        HUD_COLOUR_NET_PLAYER7 = 34,
        HUD_COLOUR_NET_PLAYER8 = 35,
        HUD_COLOUR_NET_PLAYER9 = 36,
        HUD_COLOUR_NET_PLAYER10 = 37,
        HUD_COLOUR_NET_PLAYER11 = 38,
        HUD_COLOUR_NET_PLAYER12 = 39,
        HUD_COLOUR_NET_PLAYER13 = 40,
        HUD_COLOUR_NET_PLAYER14 = 41,
        HUD_COLOUR_NET_PLAYER15 = 42,
        HUD_COLOUR_NET_PLAYER16 = 43,
        HUD_COLOUR_NET_PLAYER17 = 44,
        HUD_COLOUR_NET_PLAYER18 = 45,
        HUD_COLOUR_NET_PLAYER19 = 46,
        HUD_COLOUR_NET_PLAYER20 = 47,
        HUD_COLOUR_NET_PLAYER21 = 48,
        HUD_COLOUR_NET_PLAYER22 = 49,
        HUD_COLOUR_NET_PLAYER23 = 50,
        HUD_COLOUR_NET_PLAYER24 = 51,
        HUD_COLOUR_NET_PLAYER25 = 52,
        HUD_COLOUR_NET_PLAYER26 = 53,
        HUD_COLOUR_NET_PLAYER27 = 54,
        HUD_COLOUR_NET_PLAYER28 = 55,
        HUD_COLOUR_NET_PLAYER29 = 56,
        HUD_COLOUR_NET_PLAYER30 = 57,
        HUD_COLOUR_NET_PLAYER31 = 58,
        HUD_COLOUR_NET_PLAYER32 = 59,
        HUD_COLOUR_SIMPLEBLIP_DEFAULT = 60,
        HUD_COLOUR_MENU_BLUE = 61,
        HUD_COLOUR_MENU_GREY_LIGHT = 62,
        HUD_COLOUR_MENU_BLUE_EXTRA_DARK = 63,
        HUD_COLOUR_MENU_YELLOW = 64,
        HUD_COLOUR_MENU_YELLOW_DARK = 65,
        HUD_COLOUR_MENU_GREEN = 66,
        HUD_COLOUR_MENU_GREY = 67,
        HUD_COLOUR_MENU_GREY_DARK = 68,
        HUD_COLOUR_MENU_HIGHLIGHT = 69,
        HUD_COLOUR_MENU_STANDARD = 70,
        HUD_COLOUR_MENU_DIMMED = 71,
        HUD_COLOUR_MENU_EXTRA_DIMMED = 72,
        HUD_COLOUR_BRIEF_TITLE = 73,
        HUD_COLOUR_MID_GREY_MP = 74,
        HUD_COLOUR_NET_PLAYER1_DARK = 75,
        HUD_COLOUR_NET_PLAYER2_DARK = 76,
        HUD_COLOUR_NET_PLAYER3_DARK = 77,
        HUD_COLOUR_NET_PLAYER4_DARK = 78,
        HUD_COLOUR_NET_PLAYER5_DARK = 79,
        HUD_COLOUR_NET_PLAYER6_DARK = 80,
        HUD_COLOUR_NET_PLAYER7_DARK = 81,
        HUD_COLOUR_NET_PLAYER8_DARK = 82,
        HUD_COLOUR_NET_PLAYER9_DARK = 83,
        HUD_COLOUR_NET_PLAYER10_DARK = 84,
        HUD_COLOUR_NET_PLAYER11_DARK = 85,
        HUD_COLOUR_NET_PLAYER12_DARK = 86,
        HUD_COLOUR_NET_PLAYER13_DARK = 87,
        HUD_COLOUR_NET_PLAYER14_DARK = 88,
        HUD_COLOUR_NET_PLAYER15_DARK = 89,
        HUD_COLOUR_NET_PLAYER16_DARK = 90,
        HUD_COLOUR_NET_PLAYER17_DARK = 91,
        HUD_COLOUR_NET_PLAYER18_DARK = 92,
        HUD_COLOUR_NET_PLAYER19_DARK = 93,
        HUD_COLOUR_NET_PLAYER20_DARK = 94,
        HUD_COLOUR_NET_PLAYER21_DARK = 95,
        HUD_COLOUR_NET_PLAYER22_DARK = 96,
        HUD_COLOUR_NET_PLAYER23_DARK = 97,
        HUD_COLOUR_NET_PLAYER24_DARK = 98,
        HUD_COLOUR_NET_PLAYER25_DARK = 99,
        HUD_COLOUR_NET_PLAYER26_DARK = 100,
        HUD_COLOUR_NET_PLAYER27_DARK = 101,
        HUD_COLOUR_NET_PLAYER28_DARK = 102,
        HUD_COLOUR_NET_PLAYER29_DARK = 103,
        HUD_COLOUR_NET_PLAYER30_DARK = 104,
        HUD_COLOUR_NET_PLAYER31_DARK = 105,
        HUD_COLOUR_NET_PLAYER32_DARK = 106,
        HUD_COLOUR_BRONZE = 107,
        HUD_COLOUR_SILVER = 108,
        HUD_COLOUR_GOLD = 109,
        HUD_COLOUR_PLATINUM = 110,
        HUD_COLOUR_GANG1 = 111,
        HUD_COLOUR_GANG2 = 112,
        HUD_COLOUR_GANG3 = 113,
        HUD_COLOUR_GANG4 = 114,
        HUD_COLOUR_SAME_CREW = 115,
        HUD_COLOUR_FREEMODE = 116,
        HUD_COLOUR_PAUSE_BG = 117,
        HUD_COLOUR_FRIENDLY = 118,
        HUD_COLOUR_ENEMY = 119,
        HUD_COLOUR_LOCATION = 120,
        HUD_COLOUR_PICKUP = 121,
        HUD_COLOUR_PAUSE_SINGLEPLAYER = 122,
        HUD_COLOUR_FREEMODE_DARK = 123,
        HUD_COLOUR_INACTIVE_MISSION = 124,
        HUD_COLOUR_DAMAGE = 125,
        HUD_COLOUR_PINKLIGHT = 126,
        HUD_COLOUR_PM_MITEM_HIGHLIGHT = 127,
        HUD_COLOUR_SCRIPT_VARIABLE = 128,
        HUD_COLOUR_YOGA = 129,
        HUD_COLOUR_TENNIS = 130,
        HUD_COLOUR_GOLF = 131,
        HUD_COLOUR_SHOOTING_RANGE = 132,
        HUD_COLOUR_FLIGHT_SCHOOL = 133,
        HUD_COLOUR_NORTH_BLUE = 134,
        HUD_COLOUR_SOCIAL_CLUB = 135,
        HUD_COLOUR_PLATFORM_BLUE = 136,
        HUD_COLOUR_PLATFORM_GREEN = 137,
        HUD_COLOUR_PLATFORM_GREY = 138,
        HUD_COLOUR_FACEBOOK_BLUE = 139,
        HUD_COLOUR_INGAME_BG = 140,
        HUD_COLOUR_DARTS = 141,
        HUD_COLOUR_WAYPOINT = 142,
        HUD_COLOUR_MICHAEL = 143,
        HUD_COLOUR_FRANKLIN = 144,
        HUD_COLOUR_TREVOR = 145,
        HUD_COLOUR_GOLF_P1 = 146,
        HUD_COLOUR_GOLF_P2 = 147,
        HUD_COLOUR_GOLF_P3 = 148,
        HUD_COLOUR_GOLF_P4 = 149,
        HUD_COLOUR_WAYPOINTLIGHT = 150,
        HUD_COLOUR_WAYPOINTDARK = 151,
        HUD_COLOUR_PANEL_LIGHT = 152,
        HUD_COLOUR_MICHAEL_DARK = 153,
        HUD_COLOUR_FRANKLIN_DARK = 154,
        HUD_COLOUR_TREVOR_DARK = 155,
        HUD_COLOUR_OBJECTIVE_ROUTE = 156,
        HUD_COLOUR_PAUSEMAP_TINT = 157,
        HUD_COLOUR_PAUSE_DESELECT = 158,
        HUD_COLOUR_PM_WEAPONS_PURCHASABLE = 159,
        HUD_COLOUR_PM_WEAPONS_LOCKED = 160,
        HUD_COLOUR_END_SCREEN_BG = 161,
        HUD_COLOUR_CHOP = 162,
        HUD_COLOUR_PAUSEMAP_TINT_HALF = 163,
        HUD_COLOUR_NORTH_BLUE_OFFICIAL = 164,
        HUD_COLOUR_SCRIPT_VARIABLE_2 = 165,
        HUD_COLOUR_H = 166,
        HUD_COLOUR_HDARK = 167,
        HUD_COLOUR_T = 168,
        HUD_COLOUR_TDARK = 169,
        HUD_COLOUR_HSHARD = 170,
        HUD_COLOUR_CONTROLLER_MICHAEL = 171,
        HUD_COLOUR_CONTROLLER_FRANKLIN = 172,
        HUD_COLOUR_CONTROLLER_TREVOR = 173,
        HUD_COLOUR_CONTROLLER_CHOP = 174,
        HUD_COLOUR_VIDEO_EDITOR_VIDEO = 175,
        HUD_COLOUR_VIDEO_EDITOR_AUDIO = 176,
        HUD_COLOUR_VIDEO_EDITOR_TEXT = 177,
        HUD_COLOUR_HB_BLUE = 178,
        HUD_COLOUR_HB_YELLOW = 179,
    }
}