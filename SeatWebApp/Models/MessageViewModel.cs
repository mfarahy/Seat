namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class MessageViewModel : EntityViewModel<Message>
        {
    	    public MessageViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(Message.MessagePK),typeof(int),null, isKeyIdentity:true)
            {
            }
            public MessageViewModel(Message obj)
                : base(obj)
            {
            }
    }
    
}
