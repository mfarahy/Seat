namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class NotificationLogViewModel : EntityViewModel<NotificationLog>
        {
    	    public NotificationLogViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(NotificationLog.NotificationLogPK),typeof(long),null, isKeyIdentity:true)
            {
            }
            public NotificationLogViewModel(NotificationLog obj)
                : base(obj)
            {
            }
    }
    
}
