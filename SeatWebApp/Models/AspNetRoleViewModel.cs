namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class AspNetRoleViewModel : EntityViewModel<AspNetRole>
        {
    	    public AspNetRoleViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(AspNetRole.Id),typeof(int),null, isKeyIdentity:true)
            {
            }
            public AspNetRoleViewModel(AspNetRole obj)
                : base(obj)
            {
            }
    }
    
}
