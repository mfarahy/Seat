using DocumentFormat.OpenXml.Spreadsheet;
using Exir.Framework.Common.Diagnostics;
using Spring.Data.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeatDomain.Models
{
    public partial class IndexLastDayTimeValue
    {
        public IndexLastDayTimeValue()
        {
        }
        public IndexLastDayTimeValue(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                var frags = text.Split(',');
                if (frags.Length == 2)
                {
                    TimeSpan timeOfDay;
                    string time = frags[0];
                    if (time != null && time.Length == 5)
                        time = "0" + time;
                    if (TimeSpan.TryParseExact(time, "hhmmss", Thread.CurrentThread.CurrentCulture.DateTimeFormat, out timeOfDay))
                        Dt = DateTime.Now.Date.Add(timeOfDay);
                    double value;
                    if (double.TryParse(frags[1], out value))
                        Value = value;
                }
            }
        }

        public string TimeText
        {
            get
            {
                return Dt.TimeOfDay.ToString("hh\\:mm");
            }
        }
        public string GetColor()
        {
           return Utility.GetColor((float)(ChangePercent ?? 0) / 100);
        }

    }
}
