using System.Collections.Generic;
using System.Drawing;
using GTA;
using GTA.UI;
using Font = GTA.UI.Font;

namespace NativeUI
{
    public abstract class TimerBarBase
    {
        public string Label { get; set; }
        
        public TimerBarBase(string label)
        {
            Label = label;
        }

        public virtual void Draw(int interval, Size offset)
        {
            SizeF res = UIMenu.GetScreenResolutionMantainRatio();
            Point safe = UIMenu.GetSafezoneBounds() + offset;

            new UIResText(Label, new Point((int)res.Width - safe.X - 180, (int)res.Height - safe.Y - (30 + (interval))), 0.3f, Color.White, Font.ChaletLondon, UIResText.Alignment.Right).Draw();
            new Sprite("timerbars", "all_black_bg", new Point((int)res.Width - safe.X - 298, (int)res.Height - safe.Y - (40 + (interval))), new Size(300, 37), 0f, Color.FromArgb(180, 255, 255, 255)).Draw();

			Screen.HideHudComponentThisFrame(HudComponent.AreaName);
			Screen.HideHudComponentThisFrame(HudComponent.StreetName);
			Screen.HideHudComponentThisFrame(HudComponent.VehicleName);
        }
    }

    public class TextTimerBar : TimerBarBase
    {
        public string Text { get; set; }

        public TextTimerBar(string label, string text) : base(label)
        {
            Text = text;
        }

        public override void Draw(int interval, Size offset)
        {
            SizeF res = UIMenu.GetScreenResolutionMantainRatio();
            Point safe = UIMenu.GetSafezoneBounds() + offset;

            base.Draw(interval, offset);
            new UIResText(Text, new Point((int)res.Width - safe.X - 10, (int)res.Height - safe.Y - (42 + (interval))), 0.5f, Color.White, Font.ChaletLondon, UIResText.Alignment.Right).Draw();
        }
    }

    public class BarTimerBar : TimerBarBase
    {
        public string Text { get; set; }

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

        public override void Draw(int interval, Size offset)
        {
            SizeF res = UIMenu.GetScreenResolutionMantainRatio();
            Point safe = UIMenu.GetSafezoneBounds() + offset;

            base.Draw(interval, offset);

            var start = new Point((int)res.Width - safe.X - 160, (int)res.Height - safe.Y - (28 + (interval)));

            new UIResRectangle(start, new Size(150, 15), BackgroundColor).Draw();
            new UIResRectangle(start, new Size((int)(150 * Percentage), 15), ForegroundColor).Draw();
        }
    }

    public class TimerBarPool
    {
        private List<TimerBarBase> _bars;
        
        public Size Offset { get; set; }

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
                _bars[i].Draw(i * 40, Offset);
            }
        }
    }
}