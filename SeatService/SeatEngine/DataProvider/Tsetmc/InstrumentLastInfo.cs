using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatService.SeatServiceEngine.DataProvider.Tsetmc
{
    public class InstrumentLastInfo
    {

        public string Symbole { get; internal set; }
        public string LSoc30 { get; internal set; }
        public long InsCode { get; internal set; }
        public string InstrumentID { get; internal set; }
        public float EstimatedEPS { get; internal set; }
        public long BaseVol { get; internal set; }
        public int CSecVal { get; internal set; }
        public long ZTitad { get; internal set; }
        public FlowTypes Flow { get; internal set; }
        public int PSGelStaMin { get; internal set; }
        public int PSGelStaMax { get; internal set; }
        public int MinYear { get; internal set; }
        public int MaxYear { get; internal set; }
        /// <summary>
        /// میانگین حجم معاملات
        /// </summary>
        public long QTotTran5JAvg { get; internal set; }
        public float SectorPE { get; internal set; }
        /// <summary>
        /// حجم شناور
        /// </summary>
        public long KAjCapValCpsIdx { get; internal set; }
        public int PriceMin { get; internal set; }
        public int PriceMax { get; internal set; }
        public DateTime DEven { get; internal set; }
        public InstrumentStates Status { get; internal set; }
        public int PriceYesterday { get; internal set; }
        public int Last { get; internal set; }
        public int Count { get; internal set; }
        /// <summary>
        /// قیمت آخرین معامله
        /// </summary>
        public int PdrCotVal { get; internal set; }
        public int PClosing { get; internal set; }
        public long Vol { get; internal set; }
        public decimal Val { get; internal set; }
        public decimal BVal { get; internal set; }
        public int? NAV { get; internal set; }
        public DateTime? NAVDate { get; internal set; }
    }
}
