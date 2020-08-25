using Exir.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Models
{
    public class BigDealSearchModel : SearchSpecification<BigDeal>
    {
        public BigDealSearchModel()
        {
            FromDt = DateTime.Now.Date;
            TraderType = Constants.BigDeals.TraderTypes.Individual;
        }

        public long? InsCode { get; set; }
        public byte? DealType { get; set; }
        public byte? TraderType { get; set; }
        public string CSecVal { get; set; }
        public long? MinMoney { get; set; }

        public DateTime? FromDt { get; set; }
        public DateTime? ToDt { get; set; }

    }
}
