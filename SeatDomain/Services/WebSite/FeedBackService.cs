using Exir.Framework.Common;
using SeatDomain.Models;
using SeatDomain.Services.Notifications;
using System;

namespace SeatDomain.Services
{
    public partial interface IFeedBackService : IReadOnlySupportBaseOfService<FeedBack, IFeedBackService>
    {
        ActionResponse Response(DescriptionModel model);
        ActionResponse Okay(string selectedRows);

    }

    [IgnoreT4Template]

    public partial class FeedBackService : ReadOnlySupportBaseOfService<FeedBack, IFeedBackService>, IFeedBackService
    {
        public FeedBackService(IRepository<FeedBack> repository, IReadOnlyRepository<FeedBack> readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }

        public ActionResponse Okay(string selectedRows)
        {
            var rprovider = ObjectRegistry.GetObject<IResourceProvider>();
            return Response(new DescriptionModel
            {
                SelectedRows = selectedRows,
                Description = rprovider.GetResource(Constants.Resources.Sections.Messages, Constants.Resources.Messages.feedback_okay_message)
            });
        }

        public ActionResponse Response(DescriptionModel model)
        {
            var pk = int.Parse(model.SelectedRows);
            var entity = GetEntity(pk, null);
            var rprovider = ObjectRegistry.GetObject<IResourceProvider>();
            if (String.IsNullOrEmpty(entity.Email) && String.IsNullOrEmpty(entity.Mobile))
                return new ActionResponse(rprovider.GetResource(Constants.Resources.Sections.Messages, Constants.Resources.Messages.feedback_has_no_email_and_mobile));

            entity.Status = Constants.FeedbackStates.Responsed;
            entity.Response = model.Description;
            Save(entity);

            var noty = ObjectRegistry.GetObject<INotificationProvider>();

            if (!String.IsNullOrEmpty(entity.Email))
            {
                string subject = rprovider.GetResource(Constants.Resources.Sections.Messages, Constants.Resources.Messages.feed_back_response_subject);
                string body_template = rprovider.GetResource(Constants.Resources.Sections.Messages, Constants.Resources.Messages.feed_back_response_body_template);
                body_template = StringFormatUtility.ReplaceParams(body_template, new
                {
                    UserName = entity.UserName,
                    Description = entity.Description,
                    Response = entity.Response
                }, "[[", "]]");
                noty.SendEmail( subject, body_template, entity.Email, true);
            }
            else
            {
                string sms = rprovider.GetResource(Constants.Resources.Sections.Messages, Constants.Resources.Messages.feed_back_response_sms_template);
                sms = StringFormatUtility.ReplaceParams(sms, new
                {
                    Response = entity.Response,
                }, "[[", "]]");
                noty.SendSms( sms, new[] { entity.Mobile });
            }

            return new ActionResponse(true);
        }

        public override object Save(FeedBack entity)
        {
            if (!entity.HasKey())
            {
                var authenticater = ObjectRegistry.GetObject<IAuthenticaterProvider>(true);
                entity.UserName = authenticater.CurrentIdentity.Name;
                entity.CreateDate = DateTime.Now;
                entity.Status = (int)Constants.FeedbackStates.New;
            }
            return base.Save(entity);
        }
    }

}
