using Exir.Framework.Uie.Adapter;
using SeatDomain.Models.Customized;
using System;

namespace SeatWebApp.Models
{
    public partial class CacheReportViewModel : EntityViewModel<CacheReport>
    {
        public CacheReportViewModel(object obj, string pk, Type pkType, string version)
            : base(obj, nameof(CacheReport.Key), typeof(string), null, isKeyIdentity: true)
        {
        }
        public CacheReportViewModel(CacheReport obj)
            : base(obj)
        {
        }
    }

}
