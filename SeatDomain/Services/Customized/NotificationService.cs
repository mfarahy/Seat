using Exir.Framework.Common;
using SeatDomain.Models;
using Exir.Framework.Service.Auditer;
using Exir.Framework.Common.Linq;
using System.Linq;

namespace SeatDomain.Services
{
    public partial interface INotificationService : IAuditerService<Notification> {
        ActionResponse CanAddNotification();
    }

    [IgnoreT4Template]
    public partial class NotificationService : ReadOnlySupportAuditerService<Notification, IRepository<Notification>, IReadOnlyRepository<Notification>, INotificationService>, INotificationService
    {

        protected new INotificationService This { get { return base.This<INotificationService>(); } }

        public NotificationService(IRepository<Notification> repository, IReadOnlyRepository<Notification> readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }

        public override object Save(Notification entity)
        {
            if (!entity.HasKey())
            {
                entity.Owner = Authenticater.Value.CurrentIdentity.Name.ToLower();
            }
            return base.Save(entity);
        }

        public ActionResponse CanAddNotification()
        {
            string cusername = Authenticater.Value.CurrentIdentity.Name;
            var userSrvf = ServiceFactory.Create<IAspNetUserService>();
            var user = userSrvf.FindByName(cusername);
            if (!user.EmailConfirmed && !user.PhoneNumberConfirmed)
                return new ActionResponse("تلفن و آدرس ایمیل کاربر هیچ کدام تایید نشده است.");
            var portfoSrv = ServiceFactory.Create<IPortfolioService>();
            var portfoPKs = portfoSrv.GetDefaultQuery()
                .Where(x => x.Owner.Equals(cusername, System.StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.PortfolioPK)
                .ToList();

            var nportfoPKs = GetDefaultQuery()
                .Where(x => x.Owner.Equals(cusername, System.StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.PortfolioPK)
                .ToList();
            portfoPKs.RemoveFromIList(x => nportfoPKs.Contains(x));
            if (portfoPKs.Count == 0)
                return new ActionResponse("برای تمام سبدها زنگ هشدار تعریف شده است و امکان ثبت زنگ هشدار جدید نیست.");
            return new ActionResponse(true);
        }
    }

}
