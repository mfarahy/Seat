namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class HardLogViewModel : EntityViewModel<HardLog>
        {
    	    public HardLogViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(HardLog.Id),typeof(long),null, isKeyIdentity:true)
            {
            }
            public HardLogViewModel(HardLog obj)
                : base(obj)
            {
            }
    }
    
}
