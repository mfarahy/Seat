using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatService.SeatServiceEngine.DataProvider.Tsetmc
{
    public class ColumnInfo
    {
        public int Index
        {
            get;
            set;
        }

        public ColumnType Type
        {
            get;
            set;
        }

        public string Header
        {
            get;
            set;
        }

        public bool Visible
        {
            get;
            set;
        }
    }
}
