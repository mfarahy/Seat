namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class BigDealViewModel : EntityViewModel<BigDeal>
        {
    	    public BigDealViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(BigDeal.BigDealPK),typeof(long),null, isKeyIdentity:true)
            {
            }
            public BigDealViewModel(BigDeal obj)
                : base(obj)
            {
            }
    }
    
}
