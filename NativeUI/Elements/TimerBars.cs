using System.Collections.Generic;
using System.Drawing;
using GTA;
using NativeUI.Elements;
using Font = GTA.Font;

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
            SizeF res = UIMenu.GetScreenResolutionMaintainRatio();
            Point safe = UIMenu.GetSafezoneBounds();

            new NativeText(Label, new Point((int)res.Width - safe.X - 180, (int)res.Height - safe.Y - (30 + (4 * interval))), 0.3f, Color.White, Font.ChaletLondon, TextAlignment.Right).Draw();
            new NativeSprite("timerbars", "all_black_bg", new Point((int)res.Width - safe.X - 298, (int)res.Height - safe.Y - (40 + (4 * interval))), new Size(300, 37), 0f, Color.FromArgb(180, 255, 255, 255)).Draw();

            UI.HideHudComponentThisFrame(HudComponent.AreaName);
            UI.HideHudComponentThisFrame(HudComponent.StreetName);
            UI.HideHudComponentThisFrame(HudComponent.VehicleName);
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
            SizeF res = UIMenu.GetScreenResolutionMaintainRatio();
            Point safe = UIMenu.GetSafezoneBounds();

            base.Draw(interval);
            new NativeText(Text, new Point((int)res.Width - safe.X - 10, (int)res.Height - safe.Y - (42 + (4 * interval))), 0.5f, Color.White, Font.ChaletLondon, TextAlignment.Right).Draw();
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
            BackgroundColor = Color.DarkRed;
            ForegroundColor = Color.Red;
        }

        public override void Draw(int interval)
        {
            SizeF res = UIMenu.GetScreenResolutionMaintainRatio();
            Point safe = UIMenu.GetSafezoneBounds();

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
