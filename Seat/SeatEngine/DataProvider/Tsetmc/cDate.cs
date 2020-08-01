namespace Seat.SeatEngine.DataProvider.Tsetmc
{
    // TseClient.cDate
    using System;

    public class cDate
    {
        public int calendarType;

        private string[] jalaliMonth;

        public string[] jalaliWeekDays;

        private string[] arabicMonth;

        private string[] arabicWeekDays;

        private double gregorianEPOCH;

        private double jalaliEPOCH;

        private double arabicEPOCH;

        public double jd;

        public cDate(int cTypes)
        {
            calendarType = cTypes;
            jalaliMonth = new string[12]
            {
            "ÝÑæÑÏíä",
            "ÇÑÏíÈåÔÊ",
            "ÎÑÏÇÏ",
            "ÊíÑ",
            "ãÑÏÇÏ",
            "ÔåÑíæÑ",
            "ãåÑ",
            "ÂÈÇä",
            "ÂÐÑ",
            "Ïí",
            "Èåãä",
            "ÇÓÝäÏ"
            };
            jalaliWeekDays = new string[7]
            {
            "ÔäÈå",
            "íß ÔäÈå",
            "ÏæÔäÈå",
            "Óå ÔäÈå",
            "\u008dåÇÑÔäÈå",
            "\u0081äÌ ÔäÈå",
            "ÌãÚå"
            };
            arabicMonth = new string[12]
            {
            "ãÍÑã",
            "ÕÝÑ",
            "ÑÈíÚ ÇáÇæá",
            "ÑÈíÚ ÇáËÇäí",
            "ÌãÇÏí ÇáÇæá",
            "ÌãÇÏí ÇáËÇäí",
            "ÑÌÈ",
            "ÔÚÈÇä",
            "ÑãÖÇä",
            "ÔæÇá",
            "ÐíÞÚÏå",
            "ÐíÍÌå"
            };
            arabicWeekDays = new string[7]
            {
            "ÔäÈå",
            "íß ÔäÈå",
            "ÏæÔäÈå",
            "Óå ÔäÈå",
            "\u008dåÇÑÔäÈå",
            "\u0081äÌ ÔäÈå",
            "ÌãÚå"
            };
            gregorianEPOCH = 1721426.0;
            jalaliEPOCH = 1948321.0;
            arabicEPOCH = 1948440.0;
        }

        public void fromGregorain(DateTime gDate)
        {
            fromGregorain(gDate.Year, gDate.Month, gDate.Day);
        }

        public void fromGregorain(int gYear, int gMonth, int gDay)
        {
            jd = gregorian_to_jd(gYear, gMonth, gDay);
        }

        public void fromArabic(int aYear, int aMonth, int aDay)
        {
            jd = arabic_to_jd(aYear, aMonth, aDay);
        }

        public void fromJalali(int jYear, int jMonth, int jDay)
        {
            jd = jalali_to_jd(jYear, jMonth, jDay);
        }

        public void fromJalali(string JalaliDate)
        {
            int num = JalaliDate.IndexOf("/");
            int num2 = JalaliDate.LastIndexOf("/");
            int num3 = Convert.ToInt32(JalaliDate.Substring(0, num));
            int jMonth = Convert.ToInt32(JalaliDate.Substring(num + 1, num2 - num - 1));
            int jDay = Convert.ToInt32(JalaliDate.Substring(num2 + 1));
            if (num3 < 1000)
            {
                num3 = 1300 + num3;
            }
            jd = jalali_to_jd(num3, jMonth, jDay);
        }

        private double gregorian_to_jd(int gYear, int gMonth, int gDay)
        {
            return gregorianEPOCH - 1.0 + (double)(365 * (gYear - 1)) + Math.Floor((double)((gYear - 1) / 4)) + (0.0 - Math.Floor((double)((gYear - 1) / 100))) + Math.Floor((double)((gYear - 1) / 400)) + Math.Floor((double)((367 * gMonth - 362) / 12 + ((gMonth > 2) ? (leap_gregorian(gYear) ? (-1) : (-2)) : 0) + gDay));
        }

        private double arabic_to_jd(int aYear, int aMonth, int aDay)
        {
            return (double)aDay + Math.Ceiling(29.5 * (double)(aMonth - 1)) + (double)((aYear - 1) * 354) + Math.Floor((double)((3 + 11 * aYear) / 30)) + arabicEPOCH - 1.0;
        }

        private double jalali_to_jd(int jYear, int jMonth, int jDay)
        {
            double num = jYear - ((jYear >= 0) ? 474 : 473);
            double num2 = 474.0 + num % 2820.0;
            return (double)(jDay + ((jMonth <= 7) ? ((jMonth - 1) * 31) : ((jMonth - 1) * 30 + 6))) + Math.Floor((num2 * 682.0 - 110.0) / 2816.0) + (num2 - 1.0) * 365.0 + Math.Floor(num / 2820.0) * 1029983.0 + (jalaliEPOCH - 1.0);
        }

        private static bool leap_gregorian(int gYear)
        {
            if (gYear % 4 != 0)
            {
                return false;
            }
            if (gYear % 100 == 0)
            {
                return gYear % 400 == 0;
            }
            return true;
        }

        public DateTime ToDateTime()
        {
            int[] array = ToInt();
            return new DateTime(array[0], array[1], array[2]);
        }

        public string ToShortString()
        {
            int[] array = ToInt();
            switch (calendarType)
            {
                case 1:
                    return DateTime.MinValue.AddYears(array[0] - 1).AddMonths(array[1] - 1).AddDays(array[2] - 1)
                        .ToShortDateString();
                case 2:
                case 3:
                    return array[0] + "/" + array[1] + "/" + array[2];
                default:
                    return "";
            }
        }

        public string ToLongString()
        {
            return ToShortString();
        }

        public int[] ToInt()
        {
            switch (calendarType)
            {
                case 1:
                    return jd_to_gregorian();
                case 2:
                    return jd_to_jalali();
                case 3:
                    return jd_to_arabic();
                default:
                    return new int[3];
            }
        }

        private int[] jd_to_gregorian()
        {
            DateTime dateTime = new DateTime(2005, 9, 20).AddDays(jd - 2453634.0);
            return new int[3]
            {
            dateTime.Year,
            dateTime.Month,
            dateTime.Day
            };
        }

        private int[] jd_to_jalali()
        {
            double num = Math.Floor(jd) + 0.5;
            double num2 = num - jalali_to_jd(475, 1, 1);
            double num3 = Math.Floor(num2 / 1029983.0);
            double num4 = num2 % 1029983.0;
            double num5;
            if (num4 == 1029982.0)
            {
                num5 = 2820.0;
            }
            else
            {
                double num6 = Math.Floor(num4 / 366.0);
                double num7 = num4 % 366.0;
                num5 = Math.Floor((2134.0 * num6 + 2816.0 * num7 + 2815.0) / 1028522.0) + num6 + 1.0;
            }
            int num8 = (int)(num5 + 2820.0 * num3 + 474.0);
            if (num8 <= 0)
            {
                num8--;
            }
            double num9 = num - jalali_to_jd(num8, 1, 1) + 0.5;
            int num10 = (num9 <= 186.0) ? ((int)Math.Ceiling(num9 / 31.0)) : ((int)Math.Ceiling((num9 - 6.0) / 30.0));
            int num11 = (int)(num - jalali_to_jd(num8, num10, 1)) + 1;
            if (num10 == 0 && num11 == 31)
            {
                num8--;
                num10 = 12;
                num11 = 30;
            }
            return new int[3]
            {
            num8,
            num10,
            num11
            };
        }

        private int[] jd_to_arabic()
        {
            double num = Math.Floor(jd) + 0.5;
            int num2 = (int)Math.Floor((30.0 * (num - arabicEPOCH) + 10646.0) / 10631.0);
            int num3 = (int)Math.Min(12.0, Math.Ceiling((num - (29.0 + arabic_to_jd(num2, 1, 1))) / 29.5) + 1.0);
            int num4 = (int)(num - arabic_to_jd(num2, num3, 1) + 1.0);
            return new int[3]
            {
            num2,
            num3,
            num4
            };
        }
    }

}
