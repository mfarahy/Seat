namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class ShortLinkViewModel : EntityViewModel<ShortLink>
        {
    	    public ShortLinkViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(ShortLink.ShortLinkPK),typeof(int),null, isKeyIdentity:true)
            {
            }
            public ShortLinkViewModel(ShortLink obj)
                : base(obj)
            {
            }
    }
    
}
