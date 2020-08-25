namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class BestLimitViewModel : EntityViewModel<BestLimit>
        {
    	    public BestLimitViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(BestLimit.BestLimitPK),typeof(long),null, isKeyIdentity:true)
            {
            }
            public BestLimitViewModel(BestLimit obj)
                : base(obj)
            {
            }
    }
    
}
