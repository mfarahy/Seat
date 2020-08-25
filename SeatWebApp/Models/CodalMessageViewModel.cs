namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class CodalMessageViewModel : EntityViewModel<CodalMessage>
        {
    	    public CodalMessageViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(CodalMessage.TracingNo),typeof(int),null, isKeyIdentity:false)
            {
            }
            public CodalMessageViewModel(CodalMessage obj)
                : base(obj)
            {
            }
    }
    
}
