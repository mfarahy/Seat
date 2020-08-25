namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class TradeViewModel : EntityViewModel<Trade>
        {
    	    public TradeViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(Trade.TradePK),typeof(long),null, isKeyIdentity:true)
            {
            }
            public TradeViewModel(Trade obj)
                : base(obj)
            {
            }
    }
    
}
