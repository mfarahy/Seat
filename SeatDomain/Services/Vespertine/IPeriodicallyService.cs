using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exir.Framework.Common;

namespace SeatDomain.Services.Periodically
{
    public interface IPeriodicallyService : IService
    {
        TimeSpan GetPeriod();
        TimeSpan GetPeriodicallyExecutionTimeOfDay();
        DateTime GetBeginingExecutionTime();
        void ExecutePeriodicallyJob(DateTime lastExecution, DateTime date);
        /// <summary>
        /// مشخص می کند که جاب برای تاریخ جاری (تاریخ اجرای جاب) اجرا می شود یا خیر
        /// </summary>
        /// <returns></returns>
        bool IsContainCurrentDate();
    }
}
