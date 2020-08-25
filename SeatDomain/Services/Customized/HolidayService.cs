namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Common.Caching;
    using Exir.Framework.Service;
    using SeatDomain.Models;
    using System;
    using System.Linq;

    public partial interface IHolidayService
    {
        int HolidaysCountBetweenTwoDate(DateTime fromDate, DateTime toDate, bool includeFriday = false);
        int GetWorkingDaysWithHoliday(int workingDays, DateTime currentDate);
        bool IsWorkingDay(DateTime date);
        DateTime GetRecentlyWorkingDay();
    }

    public partial class HolidayService
    {
        public HolidayService(IRepository<Holiday> repository, IReadOnlyRepository<Holiday> readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }
        [Cacheable(CacheName = CacheConstants.InMemoryCache)]

        public int HolidaysCountBetweenTwoDate(DateTime fromDate, DateTime toDate, bool includeFriday = false)
        {
            var holidaysCount = This.GetAll().Count(x => x.Date.Date >= fromDate.Date && x.Date.Date <= toDate.Date);
            if (includeFriday)
                holidaysCount += CountDays(DayOfWeek.Friday, fromDate.Date, toDate.Date);

            return holidaysCount;
        }


        [Cacheable(CacheName = CacheConstants.InMemoryCache)]
        private int CountDays(DayOfWeek day, DateTime start, DateTime end)
        {
            TimeSpan ts = end - start;                       // Total duration
            int count = (int)Math.Floor(ts.TotalDays / 7);   // Number of whole weeks
            int remainder = (int)(ts.TotalDays % 7);         // Number of remaining days
            int sinceLastDay = (int)(end.DayOfWeek - day);   // Number of days since last [day]

            if (sinceLastDay < 0) sinceLastDay += 7;         // Adjust for negative days since last [day]


            // If the days in excess of an even week are greater than or equal to the number days since the last [day], then count this one, too.
            if (remainder >= sinceLastDay) count++;

            return count;
        }

        [Cacheable(CacheName = CacheConstants.InMemoryCache)]
        public int GetWorkingDaysWithHoliday(int workingDays, DateTime currentDate)
        {
            var holidays = This.GetAll();
            workingDays = Math.Abs(workingDays);
            var daysIndex = 0;
            var workingDaysIndex = 0;
            while (workingDaysIndex < workingDays)
            {
                var date = currentDate.AddDays(-1 * daysIndex);
                var isHolidayOrFriday = date.DayOfWeek == DayOfWeek.Friday || holidays.Any(e => e.Date.Date == date.Date);
                if (!isHolidayOrFriday) workingDaysIndex++;
                daysIndex++;
            }
            return daysIndex;
        }

        [JustReadOnly]
        public override Holiday[] GetAll()
        {
            base.AsReadOnly();
            return base.GetAll();
        }

        [JustReadOnly]
        [Cacheable(CacheName = CacheConstants.InMemoryCache)]
        public bool IsWorkingDay(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Thursday)
                return false;
            var holidays = This.GetAll();
            return !holidays.Any(x => x.Date == date.Date);
        }
        [JustReadOnly]
        [Cacheable(CacheName = CacheConstants.InMemoryCache)]
        public DateTime GetRecentlyWorkingDay()
        {
            var date = DateTime.Now.Date;
            while (!IsWorkingDay(date)) date = date.AddDays(-1);
            return date;
        }
    }


}
