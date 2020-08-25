namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class FeedBackViewModel : EntityViewModel<FeedBack>
        {
    	    public FeedBackViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(FeedBack.Id),typeof(int),null, isKeyIdentity:true)
            {
            }
            public FeedBackViewModel(FeedBack obj)
                : base(obj)
            {
            }
    }
    
}
