using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seat.Models
{
    public class InsDeven
    {
        public InsDeven(long insCode, int deven)
        {
            InsCode = insCode;
            Deven = deven;
        }
        public long InsCode { get; }
        public int Deven { get; }
    }
}
