using System.Collections.Generic;
using Font = CitizenFX.Core.UI.Font;
using CitizenFX.Core.UI;
using System.Drawing;
using CitizenFX.Core.Native;


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
            PointF safe = UIMenu.GetSafezoneBounds();
            new UIResText(Label, new PointF((int)res.Width - safe.X - 180, (int)res.Height - safe.Y - (30 + (4 * interval))), 0.3f, UnknownColors.White, Font.ChaletLondon, UIResText.Alignment.Right).Draw();

            new Sprite("timerbars", "all_black_bg", new PointF((int)res.Width - safe.X - 298, (int)res.Height - safe.Y - (40 + (4 * interval))), new SizeF(300, 37), 0f, Color.FromArgb(180, 255, 255, 255)).Draw();

            Screen.Hud.HideComponentThisFrame(HudComponent.AreaName);
            Screen.Hud.HideComponentThisFrame(HudComponent.StreetName);
            Screen.Hud.HideComponentThisFrame(HudComponent.VehicleName);
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
            PointF safe = UIMenu.GetSafezoneBounds();

            base.Draw(interval);
            new UIResText(Text, new PointF((int)res.Width - safe.X - 10, (int)res.Height - safe.Y - (42 + (4 * interval))), 0.5f, UnknownColors.White, Font.ChaletLondon, UIResText.Alignment.Right).Draw();
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
            BackgroundColor = UnknownColors.DarkRed;
            ForegroundColor = UnknownColors.Red;
        }

        public override void Draw(int interval)
        {
            SizeF res = UIMenu.GetScreenResolutionMaintainRatio();
            PointF safe = UIMenu.GetSafezoneBounds();

            base.Draw(interval);

            var start = new PointF((int)res.Width - safe.X - 160, (int)res.Height - safe.Y - (28 + (4 * interval)));

            new UIResRectangle(start, new SizeF(150, 15), BackgroundColor).Draw();
            new UIResRectangle(start, new SizeF((int)(150 * Percentage), 15), ForegroundColor).Draw();
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
