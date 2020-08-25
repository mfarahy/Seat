namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class AspNetUserViewModel : EntityViewModel<AspNetUser>
        {
    	    public AspNetUserViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(AspNetUser.Id),typeof(int),null, isKeyIdentity:true)
            {
            }
            public AspNetUserViewModel(AspNetUser obj)
                : base(obj)
            {
            }
    }
    
}
