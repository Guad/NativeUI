using System.Drawing;

namespace NativeUI.PauseMenu
{
    public class TabTextItem : TabItem
    {
        public string TextTitle { get; set; }
        public string Text { get; set; }
        public int WordWrap { get; set; }

        public TabTextItem(string name, string title) : base(name)
        {
            TextTitle = title;
        }

        public TabTextItem(string name, string title, string text) : base(name)
        {
            TextTitle = title;
            Text = text;
        }

        public override void Draw()
        {
            base.Draw();

            var res = UIMenu.GetScreenResolutionMaintainRatio();

            var alpha = (Focused || !CanBeFocused) ? 255 : 200;

            if (!string.IsNullOrEmpty(TextTitle))
            {
                new UIResText(TextTitle, SafeSize.AddPoints(new Point(40, 20)), 1.5f, Color.FromArgb(alpha, Color.White)).Draw();
            }

            if (!string.IsNullOrEmpty(Text))
            {
                var ww = WordWrap == 0 ? BottomRight.X - TopLeft.X - 40 : WordWrap;

                new UIResText(Text, SafeSize.AddPoints(new Point(40, 150)), 0.4f, Color.FromArgb(alpha, Color.White))
                {
                    WordWrap = new Size((int)ww, 0)
                }.Draw();
            }
        }
    }
}