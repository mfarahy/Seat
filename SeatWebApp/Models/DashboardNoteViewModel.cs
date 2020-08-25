namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class DashboardNoteViewModel : EntityViewModel<DashboardNote>
        {
    	    public DashboardNoteViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(DashboardNote.Id),typeof(int),null, isKeyIdentity:true)
            {
            }
            public DashboardNoteViewModel(DashboardNote obj)
                : base(obj)
            {
            }
    }
    
}
