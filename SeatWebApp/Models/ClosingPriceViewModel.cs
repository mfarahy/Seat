namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class ClosingPriceViewModel : EntityViewModel<ClosingPrice>
        {
    	    public ClosingPriceViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(ClosingPrice.ClosingPricePK),typeof(long),null, isKeyIdentity:true)
            {
            }
            public ClosingPriceViewModel(ClosingPrice obj)
                : base(obj)
            {
            }
    }
    
}
