namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class HolidayViewModel : EntityViewModel<Holiday>
        {
    	    public HolidayViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(Holiday.Id),typeof(int),null, isKeyIdentity:true)
            {
            }
            public HolidayViewModel(Holiday obj)
                : base(obj)
            {
            }
    }
    
}
