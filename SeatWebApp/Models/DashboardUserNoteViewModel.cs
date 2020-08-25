namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class DashboardUserNoteViewModel : EntityViewModel<DashboardUserNote>
        {
    	    public DashboardUserNoteViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(DashboardUserNote.Id),typeof(int),null, isKeyIdentity:true)
            {
            }
            public DashboardUserNoteViewModel(DashboardUserNote obj)
                : base(obj)
            {
            }
    }
    
}
