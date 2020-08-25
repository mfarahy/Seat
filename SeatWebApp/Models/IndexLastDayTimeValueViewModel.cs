namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class IndexLastDayTimeValueViewModel : EntityViewModel<IndexLastDayTimeValue>
        {
    	    public IndexLastDayTimeValueViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(IndexLastDayTimeValue.IndexLastDayTimeValuePK),typeof(int),null, isKeyIdentity:true)
            {
            }
            public IndexLastDayTimeValueViewModel(IndexLastDayTimeValue obj)
                : base(obj)
            {
            }
    }
    
}
