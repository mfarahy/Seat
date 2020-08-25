namespace SeatWebApp.Models
{
    using Exir.Framework.Uie.Adapter;
    using SeatDomain.Models;
    using System;
     public partial class PortfolioViewModel : EntityViewModel<Portfolio>
        {
    	    public PortfolioViewModel(object obj,string pk,Type pkType,string version)
                : base(obj,nameof(Portfolio.PortfolioPK),typeof(int),null, isKeyIdentity:true)
            {
            }
            public PortfolioViewModel(Portfolio obj)
                : base(obj)
            {
            }
    }
    
}
