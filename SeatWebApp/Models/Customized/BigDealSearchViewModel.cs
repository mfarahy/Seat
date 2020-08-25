using Exir.Framework.Common;
using Exir.Framework.Uie.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using Exir.Framework.Uie.Bocrud;
using SeatDomain.Models;
using SeatDomain.Services;

namespace SeatWebApp.Models
{
    public partial class BigDealSearchViewModel : EntityViewModel<BigDealSearchModel>
    {
        public BigDealSearchViewModel(object obj, string pk, Type pkType, string version)
            : base(obj, nameof(HelpInformation.HelpInfoPK), typeof(int), null, true)
        {
        }
        public BigDealSearchViewModel(BigDealSearchModel obj)
            : base(obj)
        {
        }

        protected override void OnShowContent(ShowContentEventArgs args)
        {
            var hSrv = StaticServiceFactory.Create<IHolidayService>();
            Container.FromDt = hSrv.GetRecentlyWorkingDay();

            base.OnShowContent(args);
        }
    }

}
