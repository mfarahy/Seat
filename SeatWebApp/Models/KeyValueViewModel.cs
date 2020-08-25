namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class KeyValueViewModel : EntityViewModel<KeyValue>
        {
    	    public KeyValueViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(KeyValue.KeyValuePK),typeof(long),null, isKeyIdentity:true)
            {
            }
            public KeyValueViewModel(KeyValue obj)
                : base(obj)
            {
            }
    }
    
}
