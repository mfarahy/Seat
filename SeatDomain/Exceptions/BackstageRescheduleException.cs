using Exir.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain
{
    public class BackstageException : ExirException
    {
        public DateTime Reschedule { get; private set; }
        public string RedependentCode { get; private set; }
        public int MaxRetryCount { get; set; }

        public BackstageException(DateTime reschedule) : base()
        {
            Reschedule = reschedule;
        }
        public BackstageException(string redependentCode) : base()
        {
            RedependentCode = redependentCode;
        }
        public BackstageException(string message, DateTime reschedule) : base(message)
        {
            Reschedule = reschedule;
        }
        public BackstageException(string message, string redependentCode) : base(message)
        {
            RedependentCode = redependentCode;
        }
    }
}
