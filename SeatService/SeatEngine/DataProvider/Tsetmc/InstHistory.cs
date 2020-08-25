using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatService.SeatServiceEngine.DataProvider.Tsetmc
{
    public class InstHistory
    {
        public int PClosing { get; internal set; } // قیمت پایانی
        public int PDrCotVal { get; internal set; } // قیمت آخرین معامله
        public long ZTotTran { get; internal set; } // تعداد معاملات
        public long QTotTran5J { get; internal set; } // حجم معاملات
        public long QTotCap { get; internal set; } // ارزش معاملات
        public int PriceMin { get; internal set; } // حداقل قیمت
        public int PriceMax { get; internal set; } // حداکثر قیمت
        public int PriceYesterday { get; internal set; } // قیمت روز قبل
        public int PriceFirst { get; internal set; } // قیمت آغازین
    }
}
