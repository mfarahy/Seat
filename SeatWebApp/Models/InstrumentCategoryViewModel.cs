namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class InstrumentCategoryViewModel : EntityViewModel<InstrumentCategory>
        {
    	    public InstrumentCategoryViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(InstrumentCategory.InstrumentCategoryPK),typeof(int),null, isKeyIdentity:true)
            {
            }
            public InstrumentCategoryViewModel(InstrumentCategory obj)
                : base(obj)
            {
            }
    }
    
}
