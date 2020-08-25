namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class ShareHolderChangeViewModel : EntityViewModel<ShareHolderChange>
        {
    	    public ShareHolderChangeViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(ShareHolderChange.ShareHolderChangePK),typeof(long),null, isKeyIdentity:true)
            {
            }
            public ShareHolderChangeViewModel(ShareHolderChange obj)
                : base(obj)
            {
            }
    }
    
}
