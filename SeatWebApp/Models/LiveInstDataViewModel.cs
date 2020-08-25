namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class LiveInstDataViewModel : EntityViewModel<LiveInstData>
        {
    	    public LiveInstDataViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(LiveInstData.LiveInstDataPK),typeof(long),null, isKeyIdentity:true)
            {
            }
            public LiveInstDataViewModel(LiveInstData obj)
                : base(obj)
            {
            }
    }
    
}
