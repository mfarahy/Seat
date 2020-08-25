namespace SeatWebApp.Models
{
    using Exir.Framework.Common;
    using Exir.Framework.Uie.Adapter;
    using Exir.Framework.Uie.Bocrud;
    using SeatDomain.Models;
    using SeatDomain.Services;
    using SeatWebApp.Services;
    using System;
    public partial class NotificationViewModel : EntityViewModel<Notification>
    {
        public NotificationViewModel(object obj, string pk, Type pkType, string version)
            : base(obj, nameof(Notification.NotificationPK), typeof(int), null, isKeyIdentity: true)
        {
        }
        public NotificationViewModel(Notification obj)
            : base(obj)
        {
        }

        protected override void OnPreDatabindForSave(DispatcherEventArgs args)
        {
            base.OnPreDatabindForSave(args);
        }
        public bool PhoneNumberConfirmed { get; set; }
        public bool EmailConfirmed { get; set; }
        protected override void OnShowContent(ShowContentEventArgs args)
        {
            if (!Container.HasKey())
            {
                var authenticater = ObjectRegistry.GetObject<IAuthenticaterProvider>();
                var userSrv = ServiceFactory.Create<IAspNetUserService>();
                var user = userSrv.FindByName(authenticater.CurrentIdentity.Name);
                if (user.EmailConfirmed)
                    Container.Email = user.Email;
                if (user.PhoneNumberConfirmed)
                    Container.PhoneNo = user.PhoneNumber;
                PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                EmailConfirmed = user.EmailConfirmed;
            }
            base.OnShowContent(args);
        }
    }

}
