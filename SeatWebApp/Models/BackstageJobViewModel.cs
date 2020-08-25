namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class BackstageJobViewModel : EntityViewModel<BackstageJob>
        {
    	    public BackstageJobViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(BackstageJob.BackstageJobPK),typeof(long),null, isKeyIdentity:true)
            {
            }
            public BackstageJobViewModel(BackstageJob obj)
                : base(obj)
            {
            }
    }
    
}
