using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seat.Models
{
    public partial class Instrument
    {
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
    }
}
