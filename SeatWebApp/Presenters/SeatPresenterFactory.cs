using System;
using Exir.Framework.Common.Fasterflect;
using Exir.Framework.Uie.Contracts;

namespace SeatWebApp.Presenters
{
    public class SeatPresenterFactory : Exir.Framework.Uie.Adapter.PresenterFactory
    {
        public SeatPresenterFactory() : base(
            "SeatWebApp.Presenters.SeatPresenter",
            "SeatWebApp.Presenters.SeatTreePresenter",
            "SeatWebApp")
        {

        }

        public override IPresenter CreateInstance(string serviceName, Type modelType, Type viewModelType)
        {
            return base.CreateInstance(serviceName, modelType, viewModelType);
        }
    }
}