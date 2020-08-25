namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class HistoryViewModel : EntityViewModel<History>
        {
    	    public HistoryViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(History.HistoryPK),typeof(long),null, isKeyIdentity:true)
            {
            }
            public HistoryViewModel(History obj)
                : base(obj)
            {
            }
    }
    
}
