namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class InstrumentViewModel : EntityViewModel<Instrument>
        {
    	    public InstrumentViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(Instrument.InsCode),typeof(long),null, isKeyIdentity:false)
            {
            }
            public InstrumentViewModel(Instrument obj)
                : base(obj)
            {
            }
    }
    
}
