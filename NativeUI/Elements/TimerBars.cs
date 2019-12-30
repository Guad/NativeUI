using CitizenFX.Core.UI;
using System.Collections.Generic;
using System.Drawing;

namespace NativeUI
{
    public abstract class TimerBarBase
    {
        public string Label { get; set; }

        public TimerBarBase(string label)
        {
            Label = label;
        }

        public virtual void Draw(int interval)
        {
            SizeF res = Screen.ResolutionMaintainRatio;
            Point safe = Screen.SafezoneBounds;

            UIResText.Draw(Label, (int)res.Width - safe.X - 180, (int)res.Height - safe.Y - (30 + (4 * interval)), Font.ChaletLondon, 0.3f, Colors.White,
                Alignment.Right, false, false, 0);
            Sprite.Draw("timerbars", "all_black_bg", (int)res.Width - safe.X - 298, (int)res.Height - safe.Y - (40 + (4 * interval)), 300, 37, 0f, Color.FromArgb(180, 255, 255, 255));

            CitizenFX.Core.UI.Screen.Hud.HideComponentThisFrame(HudComponent.AreaName);
            CitizenFX.Core.UI.Screen.Hud.HideComponentThisFrame(HudComponent.StreetName);
            CitizenFX.Core.UI.Screen.Hud.HideComponentThisFrame(HudComponent.VehicleName);
        }
    }

    public class TextTimerBar : TimerBarBase
    {
        public string Text { get; set; }

        public TextTimerBar(string label, string text) : base(label)
        {
            Text = text;
        }

        public override void Draw(int interval)
        {
            SizeF res = Screen.ResolutionMaintainRatio;
            Point safe = Screen.SafezoneBounds;

            base.Draw(interval);
            UIResText.Draw(Text, (int)res.Width - safe.X - 10, (int)res.Height - safe.Y - (42 + (4 * interval)), Font.ChaletLondon, 0.5f, Colors.White, 
                Alignment.Right, false, false, 0);
        }
    }

    public class BarTimerBar : TimerBarBase
    {
        /// <summary>
        /// Bar percentage. Goes from 0 to 1.
        /// </summary>
        public float Percentage { get; set; }

        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }

        public BarTimerBar(string label) : base(label)
        {
            BackgroundColor = Colors.DarkRed;
            ForegroundColor = Colors.Red;
        }

        public override void Draw(int interval)
        {
            SizeF res = Screen.ResolutionMaintainRatio;
            Point safe = Screen.SafezoneBounds;

            base.Draw(interval);

            var start = new Point((int)res.Width - safe.X - 160, (int)res.Height - safe.Y - (28 + (4 * interval)));

            UIResRectangle.Draw(start.X, start.Y, 150, 15, BackgroundColor);
            UIResRectangle.Draw(start.X, start.Y, (int)(150 * Percentage), 15, ForegroundColor);
        }
    }

    public class TimerBarPool
    {
        private static List<TimerBarBase> _bars = new List<TimerBarBase>();

        public TimerBarPool()
        {
            _bars = new List<TimerBarBase>();
        }

        public List<TimerBarBase> ToList()
        {
            return _bars;
        }

        public void Add(TimerBarBase timer)
        {
            _bars.Add(timer);
        }

        public void Remove(TimerBarBase timer)
        {
            _bars.Remove(timer);
        }

        public void Draw()
        {
            for (int i = 0; i < _bars.Count; i++)
            {
                _bars[i].Draw(i * 10);
            }
        }
    }
}
