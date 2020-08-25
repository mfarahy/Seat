using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SeatService.SeatServiceEngine.DataProvider.Tsetmc
{
    public class TsetmcDataRow
    {
        public bool Persisted { get; set; }
        public long RowId { get; set; }

        #region prev
        public int prev_heven { get; set; }
        public int prev_pf { get; set; }
        public int prev_pc { get; set; }
        public int prev_pcc { get; set; }
        public float prev_pcp { get; set; }
        public int prev_pl { get; set; }
        public int prev_plc { get; set; }
        public float prev_plp { get; set; }
        public int prev_tno { get; set; }
        public long prev_tvol { get; set; }
        public long prev_tval { get; set; }
        public int prev_pmin { get; set; }
        public int prev_pmax { get; set; }
        public float prev_pe { get; set; }
        public int prev_Buy_CountI { get; set; }
        public int prev_Buy_CountN { get; set; }
        public long prev_Buy_I_Volume { get; set; }
        public long prev_Buy_N_Volume { get; set; }
        public int prev_Sell_CountI { get; set; }
        public int prev_Sell_CountN { get; set; }
        public long prev_Sell_I_Volume { get; set; }
        #endregion
        internal bool IsValid()
        {
            if (String.IsNullOrEmpty(iid)) return false;
            if (String.IsNullOrEmpty(l18)) return false;
            if (inscode == 0) return false;

            return true;
        }

      

        internal bool IsTypeClientChanges()
        {
            if (tvol <= prev_tvol && tno <= prev_tno) return false;

            if (Buy_CountI > prev_Buy_CountI) return true;
            if (Buy_CountN > prev_Buy_CountN) return true;
            if (Buy_I_Volume > prev_Buy_I_Volume) return true;
            if (Buy_N_Volume > prev_Buy_N_Volume) return true;
            if (Sell_CountI > prev_Sell_CountI) return true;
            if (Sell_CountN > prev_Sell_CountN) return true;
            if (Sell_I_Volume > prev_Sell_I_Volume) return true;
            if (Sell_N_Volume > prev_Sell_N_Volume) return true;
            return false;
        }

        internal void Backup()
        {
            prev_Buy_CountI = Buy_CountI;
            prev_Buy_CountN = Buy_CountN;
            prev_Buy_I_Volume = Buy_I_Volume;
            prev_Buy_N_Volume = Buy_N_Volume;
            prev_Sell_CountI = Sell_CountI;
            prev_Sell_CountN = Sell_CountN;
            prev_Sell_I_Volume = Sell_I_Volume;
            prev_Sell_N_Volume = Sell_N_Volume;
            prev_heven = heven;
            prev_pf = pf;
            prev_pc = pc;
            prev_pcc = pcc;
            prev_pcp = pcp;
            prev_pl = pl;
            prev_plc = plc;
            prev_plp = plp;
            prev_tno = tno;
            prev_tvol = tvol;
            prev_tval = tval;
            prev_pmin = pmin;
            prev_pmax = pmax;
            prev_pe = pe;

        }

        internal bool IsTypeClientChangeValid()
        {
            if (Buy_CountI < prev_Buy_CountI) return false;
            if (Buy_CountN < prev_Buy_CountN) return false;
            if (Buy_I_Volume < prev_Buy_I_Volume) return false;
            if (Buy_N_Volume < prev_Buy_N_Volume) return false;
            if (Sell_CountI < prev_Sell_CountI) return false;
            if (Sell_CountN < prev_Sell_CountN) return false;
            if (Sell_I_Volume < prev_Sell_I_Volume) return false;
            if (Sell_N_Volume < prev_Sell_N_Volume) return false;
            return true;
        }

        public long prev_Sell_N_Volume { get; set; }

        public TsetmcDataRow()
        {
            History = new ConcurrentDictionary<int, InstHistory>();
        }

        public ConcurrentDictionary<int, InstHistory> History { get; set; }

        public TimeSpan LastTradeTime
        {
            get
            {
                if (heven > 0)
                {
                    var second = (int)Math.Floor(heven % 100.0);
                    var minute = (int)Math.Floor(heven / 100.0 % 100);
                    var hour = (int)Math.Floor(heven / 10000.0);

                    return new TimeSpan(hour, minute, second);
                }
                return TimeSpan.Zero;
            }
        }
        public int heven { get; set; } // زمان آخرین معامله
        public int pf { get; set; } // اولین قیمت
        public int pc { get; set; } // قیمت پایانی
        public int pcc { get; set; } // تغییر قیمت پایانی
        public float pcp { get; set; } // درصد تغییر قیمت پایانی
        public int pl { get; set; } // آخرین قیمت
        public int plc { get; set; } // تغییر آخرین قیمت
        public float plp { get; set; } // درصد تغییر آخرین قیمت
        public int tno { get; set; } // تعداد معاملات
        public long tvol { get; set; }  // حجم معاملات
        public long tval { get; set; } // ارزش معاملات
        public int pmin { get; set; } // کمترین قیمت
        public int pmax { get; set; } // بیشترین قیمت
        public float eps { get; set; }
        public float pe { get; set; }
        public string l18 { get; set; } // نماد
        public string l30 { get; set; } // نام
        public int py { get; set; } // قیمت دیروز
        public long bvol { get; set; }
        public int visitcount { get; set; } // تعداد بازدید - پربیننده
        public FlowTypes flow { get; set; }
        public int cs { get; set; }
        public int tmax { get; set; } // آستانه مجاز بالا
        public int tmin { get; set; } // آستانه مجاز پایین
        public long z { get; set; } // تعداد سهام
        public string yval { get; set; } // 

        public int zd1 { get; set; } // تعداد خریدار
        public int pd1 { get; set; } // قیمت خرید
        public long qd1 { get; set; } // حجم خرید 

        public int po1 { get; set; } // قیمت فروش
        public long qo1 { get; set; } // حجم فروش
        public int zo1 { get; set; }// تعداد فروشنده
        
        public int zo2 { get; set; } 
        public int zd2 { get; set; }
        public int pd2 { get; set; }
        public int po2 { get; set; }
        public long qd2 { get; set; }
        public long qo2 { get; set; }
        public int zo3 { get; set; }
        public int zd3 { get; set; }
        public int pd3 { get; set; }
        public int po3 { get; set; }
        public long qd3 { get; set; }
        public long qo3 { get; set; }
        public long inscode { get; set; }
        public string iid { get; set; }
        public int Buy_CountI { get; set; }
        public int Buy_CountN { get; set; }
        public long Buy_I_Volume { get; set; }
        public long Buy_N_Volume { get; set; }
        public int Sell_CountI { get; set; }
        public int Sell_CountN { get; set; }
        public long Sell_I_Volume { get; set; }
        public long Sell_N_Volume { get; set; }

        public DateTime DEven { get; internal set; }

        public override string ToString()
        {
            return l18;
        }
    }
}