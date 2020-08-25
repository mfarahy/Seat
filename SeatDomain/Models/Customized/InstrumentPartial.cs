using Exir.Framework.Common;
using log4net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SeatDomain.Constants;

namespace SeatDomain.Models
{
    public partial class Instrument
    {
        public Instrument()
        {
        }


        public int LastDbDeven { get; set; }
        public int LastDeven { get; set; }
        internal bool IsValid()
        {
            if (InsCode == 0) return false;
            if (String.IsNullOrEmpty(Symbol)) return false;
            if (String.IsNullOrEmpty(Name)) return false;
            if (String.IsNullOrEmpty(LatinName)) return false;
            if (String.IsNullOrEmpty(LatinSymbol)) return false;

            return true;
        }

        public string DevenText
        {
            get
            {
                DateTime date;
                if (DateTime.TryParseExact(DEven.ToString(), "yyyyMMdd", Thread.CurrentThread.CurrentCulture.DateTimeFormat, System.Globalization.DateTimeStyles.None, out date))
                    return date.ToPersian().GetShortDate();
                return String.Empty;
            }
        }
        public string InstrumentText
        {
            get
            {
                switch (InstrumentType)
                {
                    case InstrumentTypes.Ati: return InstrumentTextTypes.Ati;
                    case InstrumentTypes.EkhtiarForoush: return InstrumentTextTypes.EkhtiarForoush;
                    case InstrumentTypes.HaghTaghaddom: return InstrumentTextTypes.HaghTaghaddom;
                    case InstrumentTypes.HousingFacilities: return InstrumentTextTypes.HousingFacilities;
                    case InstrumentTypes.Kala: return InstrumentTextTypes.Kala;
                    case InstrumentTypes.OraghMosharekat: return InstrumentTextTypes.OraghMosharekat;
                    case InstrumentTypes.PayeFarabourse: return InstrumentTextTypes.PayeFarabourse;
                    case InstrumentTypes.Saham: return InstrumentTextTypes.Saham;
                    case InstrumentTypes.Sandoogh: return InstrumentTextTypes.Sandoogh;
                    case InstrumentTypes.unknown: return InstrumentTextTypes.unknown;
                }
                return String.Empty;
            }
        }
        public InstrumentTypes InstrumentType
        {
            get
            {
                if (Symbol != null && (Symbol.IndexOf("تسه") == 0 || Symbol.IndexOf("تملي") == 0))
                    return InstrumentTypes.HousingFacilities;

                if ((YVal == "300" || YVal == "303" || YVal == "313") && (Symbol == null || Symbol.IndexOf("تسه") != 0))
                    return InstrumentTypes.Saham;

                if (YVal == "309")
                    return InstrumentTypes.PayeFarabourse;

                if (YVal == "400" || YVal == "403" || YVal == "404")
                    return InstrumentTypes.HaghTaghaddom;

                if (YVal == "306" || YVal == "301" || YVal == "706" || YVal == "208")
                    return InstrumentTypes.OraghMosharekat;

                if (YVal == "263")
                    return InstrumentTypes.Ati;

                if (YVal == "305" || YVal == "380")
                    return InstrumentTypes.Sandoogh;

                if (YVal == "600" || YVal == "602" || YVal == "605" || YVal == "311" || YVal == "312")
                    return InstrumentTypes.EkhtiarForoush;

                if (YVal == "308" || YVal == "701")
                    return InstrumentTypes.Kala;

                return InstrumentTypes.unknown;
            }
        }

        public double LastValue { get; set; }
        public double ChangeValue { get; set; }
        public double ChangePercent { get; set; }


    }
}
