namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class UserActivityViewModel : EntityViewModel<UserActivity>
        {
    	    public UserActivityViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(UserActivity.Id),typeof(long),null, isKeyIdentity:true)
            {
            }
            public UserActivityViewModel(UserActivity obj)
                : base(obj)
            {
            }
    }
    
}
