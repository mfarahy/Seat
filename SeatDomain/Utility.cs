using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain
{
    public static class Utility
    {
        public static string GetColor(float percent)
        {
            // -5 .. +5


            System.Drawing.Color color1 = System.Drawing.Color.Red;
            System.Drawing.Color color2 = System.Drawing.Color.Green;

            if (percent > 0)
                color1 = System.Drawing.Color.Yellow;
            else
                color2 = System.Drawing.Color.Yellow;

            // 0 .. 10
            percent = (percent + 5) / 10;

            double resultRed = Math.Max(0, Math.Min(color1.R + percent * (color2.R - color1.R), 255));
            double resultGreen = Math.Max(0, Math.Min(color1.G + percent * (color2.G - color1.G), 255));
            double resultBlue = Math.Max(0, Math.Min(color1.B + percent * (color2.B - color1.B), 255));

            return "#" + System.Drawing.Color.FromArgb((int)resultRed, (int)resultGreen, (int)resultBlue).ToArgb().ToString("x").Substring(2);
        }
    }
}
