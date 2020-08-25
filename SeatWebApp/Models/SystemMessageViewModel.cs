namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class SystemMessageViewModel : EntityViewModel<SystemMessage>
        {
    	    public SystemMessageViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(SystemMessage.MessagePK),typeof(int),null, isKeyIdentity:true)
            {
            }
            public SystemMessageViewModel(SystemMessage obj)
                : base(obj)
            {
            }
    }
    
}
