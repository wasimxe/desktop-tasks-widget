using System.Drawing;
using System.Drawing.Drawing2D;

namespace DesktopTasks.Helpers
{
    public static class IconHelper
    {
        public static Icon CreateDefaultIcon()
        {
            // Create a simple icon with a checkmark
            var bitmap = new Bitmap(32, 32);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Draw background circle
                g.FillEllipse(new SolidBrush(Color.FromArgb(0, 123, 255)), 2, 2, 28, 28);

                // Draw checkmark
                using (var pen = new Pen(Color.White, 3))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    g.DrawLines(pen, new Point[]
                    {
                        new Point(8, 16),
                        new Point(14, 22),
                        new Point(24, 10)
                    });
                }
            }

            return Icon.FromHandle(bitmap.GetHicon());
        }
    }
}
