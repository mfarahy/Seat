using Exir.Framework.Common;
using Exir.Framework.Common.Linq;
using Exir.Framework.Service.ActionResponses;
using SeatDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Services
{
    public partial interface IBigDealService
    {
        DataPageResponse<BigDeal> SearchByInsCode(long insCode, BigDealSearchModel searchModel);
        DataPageResponse<BigDeal> Search(BigDealSearchModel searchModel);
    }
    public partial class BigDealService
    {
        public BigDealService(IRepository<BigDeal> repository, IReadOnlyRepository<BigDeal> readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }
        public DataPageResponse<BigDeal> SearchByInsCode(long insCode, BigDealSearchModel searchModel)
        {
            if (searchModel == null)
                searchModel = new BigDealSearchModel();
            searchModel.InsCode = insCode;
            if (!searchModel.FromDt.HasValue)
            {
                var hSrv = ServiceFactory.Create<IHolidayService>();
                if (hSrv.IsWorkingDay(DateTime.Now.Date))
                {
                    searchModel.FromDt = GetDefaultQuery()
                        .Where(x => x.InsCode == insCode)
                        .OrderByDescending(x => x.DayDt)
                        .Select(x => x.DayDt)
                        .FirstOrDefault();
                }
                else
                    searchModel.FromDt = DateTime.Now.Date;
            }

            if (!searchModel.MinMoney.HasValue)
                searchModel.MinMoney = Constants.BigDealsMinMoney;

            return Search(searchModel);
        }
        public DataPageResponse<BigDeal> Search(BigDealSearchModel searchModel)
        {
            var predicate = PredicateBuilder.New<BigDeal>(x => true);

            if (searchModel.InsCode.HasValue)
                predicate = predicate.And(x => x.InsCode == searchModel.InsCode.Value);

            if (!String.IsNullOrEmpty(searchModel.CSecVal))
                predicate = predicate.And(x => x.CSecVal == searchModel.CSecVal);

            if (searchModel.MinMoney.HasValue)
                predicate = predicate.And(x => x.AmountAvg >= searchModel.MinMoney || x.AmountAvg >= searchModel.MinMoney);

            if (searchModel.FromDt.HasValue)
                predicate = predicate.And(x => x.DayDt >= searchModel.FromDt);

            if (searchModel.TraderType.HasValue)
                predicate = predicate.And(x => x.TraderType == searchModel.TraderType.Value);

            if (searchModel.DealType.HasValue)
                predicate = predicate.And(x => x.DealType == searchModel.DealType.Value);

            if (searchModel.ToDt.HasValue)
                predicate = predicate.And(x => x.DayDt <= searchModel.ToDt);

            var query = GetDefaultQuery().Where(predicate);

            if (searchModel.Sorting != null &&
                (searchModel.Sorting[0].Name == nameof(BigDeal.Time) ||
                searchModel.Sorting[0].Name == nameof(BigDeal.Date)))
                searchModel.Sorting[0].Name = nameof(BigDeal.DayDt);

            var count = query.Count();

            query = ApplySortingRule(query, searchModel, x => x.DayDt);
            query = ApplyPagingRule(query, searchModel);

            var data = query.ToList();

            if (!searchModel.InsCode.HasValue)
            {
                var iSrv = ServiceFactory.Create<IInstrumentService>();
                var ins = iSrv.GetAll();
                for (var i = 0; i < data.Count; ++i)
                {
                    var instrument = ins.FirstOrDefault(x => x.InsCode == data[i].InsCode);
                    if (instrument != null)
                    {
                        data[i].Symbol = instrument.Symbol;
                    }
                }
            }

            return new DataPageResponse<BigDeal>()
            {
                Data = data,
                Filterable = false,
                PageNumber = searchModel.PageNumber,
                TotalRecordCount = count,
                Success = true
            };
        }
    }
}
