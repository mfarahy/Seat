namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class NewsCategoryViewModel : EntityViewModel<NewsCategory>
        {
    	    public NewsCategoryViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(NewsCategory.CategoryPk),typeof(int),null, isKeyIdentity:true)
            {
            }
            public NewsCategoryViewModel(NewsCategory obj)
                : base(obj)
            {
            }
    }
    
}
