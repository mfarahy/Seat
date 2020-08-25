namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class ClientTypeViewModel : EntityViewModel<ClientType>
        {
    	    public ClientTypeViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(ClientType.ClintTypePK),typeof(long),null, isKeyIdentity:true)
            {
            }
            public ClientTypeViewModel(ClientType obj)
                : base(obj)
            {
            }
    }
    
}
