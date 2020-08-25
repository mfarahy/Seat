using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatService.SeatServiceEngine.DataProvider.Tsetmc
{
    // TseClient.TseShareInfo
    public class TseShareInfo
    {
        public long Idn
        {
            get;
            set;
        }

        public long InsCode
        {
            get;
            set;
        }

        public int DEven
        {
            get;
            set;
        }

        public decimal NumberOfShareNew
        {
            get;
            set;
        }

        public decimal NumberOfShareOld
        {
            get;
            set;
        }
    }

}
