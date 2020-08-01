using Seat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seat.Services
{
    public partial class BestLimitService
    {
        private static int FindBestLimitsRow(int row, int index, List<BestLimit> data)
        {
            for (int c = index; c > -1; c--)
            {
                if (data[c].Row == row)
                {
                    return c;
                }
            }
            return -1;
        }
    }
}
