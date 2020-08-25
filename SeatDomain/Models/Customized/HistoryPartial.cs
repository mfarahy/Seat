using System;

namespace SeatDomain.Models
{
    public partial class History
    {
        public History() { }
        public History(ClosingPriceInfo closingPriceInfo)
        {
            this.DEven = closingPriceInfo.DEven;
            this.InsCode = closingPriceInfo.InsCode;
            this.PClosing = closingPriceInfo.PClosing;
            this.PDrCotVal = (int)closingPriceInfo.PDrCotVal;
            this.PriceFirst = (int)closingPriceInfo.PriceFirst;
            this.PriceMax = (int)closingPriceInfo.PriceMax;
            this.PriceMin = (int)closingPriceInfo.PriceMin;
            this.PriceYesterday = closingPriceInfo.PriceYesterday;
            this.QTotCap = (long)closingPriceInfo.QTotCap;
            this.QTotTran5J = (long)closingPriceInfo.QTotTran5J;
            this.ZTotTran = (long)closingPriceInfo.ZTotTran;
            // 2020 07 22
            if (DEven > 0)
                this.Date = new DateTime(DEven / 10000, DEven / 100 % 100, DEven % 100);


            
        }
    }
}
