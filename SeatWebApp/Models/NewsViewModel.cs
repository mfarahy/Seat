namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class NewsViewModel : EntityViewModel<News>
        {
    	    public NewsViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(News.NewsPk),typeof(int),null, isKeyIdentity:true)
            {
            }
            public NewsViewModel(News obj)
                : base(obj)
            {
            }
    }
    
}
