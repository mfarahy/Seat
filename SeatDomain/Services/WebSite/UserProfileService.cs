namespace SeatDomain.Services
{
    using AutoMapper;
    using Exir.Framework.Common;
    using Exir.Framework.Common.Caching;
    using Exir.Framework.Common.Logging;
    using Exir.Framework.Common.Random;
    using Exir.Framework.Security.Cryptography;
    using Exir.Framework.Service;
    using SeatDomain.Models;
    using SeatDomain.Services.Notifications;
    using System;
    using System.Linq;
    using System.Threading;

    public partial interface IUserProfileService : IReadOnlySupportBaseOfService<UserProfile, IUserProfileService>
    {
        UserProfile GetCurrentProfile();

        byte[] GetUserProfileAvatar();
        ActionResponse ConfirmPhoneNumberCode(UserProfile profile);
        ActionResponse SendConfirmEmail(UserProfile profile);
        ActionResponse SendConfirmSms(UserProfile profile);
    }

    [IgnoreT4Template]

    public partial class UserProfileService : ReadOnlySupportBaseOfService<UserProfile, IUserProfileService>, IUserProfileService
    {
        protected new IUserProfileService This { get { return This<IUserProfileService>(); } }
        public UserProfileService(IRepository<UserProfile> repository, IReadOnlyRepository<UserProfile> readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }

        public UserProfile GetCurrentProfile()
        {
            var username = Authenticater.Value.CurrentIdentity.Name;
            var profile = GetDefaultQuery()
               .Where(x => username.Equals(x.Username, StringComparison.CurrentCultureIgnoreCase))
               .FirstOrDefault();


            if (profile == null)
            {
                profile = (UserProfile)Save(new UserProfile
                {
                    Username = username
                });
            }

            var userSrv = ServiceFactory.Create<IAspNetUserService>();
            var user = userSrv.GetCurrent();
            profile.Email = user.Email;
            profile.PhoneNumber = user.PhoneNumber;
            profile.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            profile.EmailConfirmed = user.EmailConfirmed;

            return profile;
        }
        public override object Save(UserProfile entity)
        {
            if (entity.HasKey())
            {
                var userSrv = ServiceFactory.Create<IAspNetUserService>();
                var user = userSrv.GetCurrent();
                if (String.Compare(user.Email, entity.Email, true) != 0)
                    user.EmailConfirmed = false;
                user.Email = entity.Email;
                if (String.Compare(user.PhoneNumber, entity.PhoneNumber, true) != 0)
                    user.PhoneNumberConfirmed = false;
                user.PhoneNumber = entity.PhoneNumber;
                userSrv.Save(user);
            }

            return base.Save(entity);
        }

        public ActionResponse SendConfirmEmail(UserProfile profile)
        {
            var userSrv = ServiceFactory.Create<IAspNetUserService>();
            var user = userSrv.GetCurrent();

            if (!String.IsNullOrEmpty(profile.Email))
            {
                user.Email = profile.Email;
                if (String.Compare(user.Email, profile.Email, true) != 0)
                    user.EmailConfirmed = false;

                userSrv.Save(user);
            }


            if (String.IsNullOrEmpty(user.Email))
                return new ActionResponse("آدرس ایمیل تنظیم نشده است.");

            profile.ResetChanges();
            int sendCount = profile.EmailConfirmSendCount + 1;
            if (profile.EmailConfirmSendDt != null)
            {
                var time_diff = profile.EmailTimeDiff.Value;
                if (profile.EmailExceedWaitTimeLimit)
                    return new ActionResponse(string.Format("برای ارسال مجدد تاییدیه ایمیل لطفا حداقل {0} ثانیه صبر نمایید.",
                        (profile.EmailWaitTime.Value - DateTime.Now).TotalSeconds));

                if (profile.EmailExceedSendCountLimit)
                    return new ActionResponse(string.Format("ارسال تاییدیه ایمیل بیشتر از {0} بار در طول یک روز امکانپذیر نمی باشد.",
                        Constants.Notification.MaxCount));

                if (time_diff.TotalHours > 24)
                    sendCount = 1;
            }

            var msgSrv = ServiceFactory.Create<ISystemMessageService>();
            var subject = msgSrv.GetByCode(Constants.Messages.email_confirm_subject, Constants.SystemMessage.Media.Sms, null).FirstOrDefault()?.Text.Trim();
            var body = msgSrv.GetByCode(Constants.Messages.email_confirm_body);

            var code = EmbedRsaCryptoService.XorEncryptSafeBase64(user.Id + "|" + user.Email);

            body = StringFormatUtility.ReplaceParams(body, new
            {
                username = user.UserName,
                code = code
            }, "[[", "]]");

            var noty = ObjectRegistry.GetObject<INotificationProvider>();
            var result = noty.VerifyEmail(subject, body, user.Email, true);

            if (result.Result == NotificationResultType.Success)
            {
                string username = user.UserName;
                Repository.BulkUpdate(x => username.Equals(x.Username, StringComparison.CurrentCultureIgnoreCase), x => new UserProfile
                {
                    EmailConfirmSendDt = DateTime.Now,
                    EmailConfirmSendCount = sendCount
                });
            }

            return new ActionResponse
            {
                Success = result.Result == NotificationResultType.Success,
                Message = result.Result == NotificationResultType.Success ? "تاییدیه آدرس ایمیل ارسال شد" : result.Message
            };
        }

        public ActionResponse SendConfirmSms(UserProfile profile)
        {
            var userSrv = ServiceFactory.Create<IAspNetUserService>();
            var user = userSrv.GetCurrent();
            var username = user.UserName;

            if (!String.IsNullOrEmpty(profile.PhoneNumber))
            {
                user.PhoneNumber = profile.PhoneNumber;
                if (String.Compare(user.PhoneNumber, profile.PhoneNumber, true) != 0)
                    user.PhoneNumberConfirmed = false;
                userSrv.Save(user);
            }

            if (String.IsNullOrEmpty(user.PhoneNumber))
                return new ActionResponse("شماره تماس تنظیم نشده است.");

            profile.ResetChanges();

            int sendCount = profile.PhoneNumberConfirmSendCount + 1;
            if (profile.PhoneNumberConfirmSendDt != null)
            {
                var time_diff = profile.PhoneTimeDiff.Value;
                if (profile.PhoneExceedWaitTimeLimit)
                    return new ActionResponse(string.Format("برای ارسال پیامک تایید لطفا حداقل {0} ثانیه صبر نمایید.",
                        (profile.PhoneWaitTime.Value - DateTime.Now).TotalSeconds));

                if (profile.PhoneExceedSendCountLimit)
                    return new ActionResponse(string.Format("ارسال پیامک تایید بیشتر از {0} بار در طول یک روز امکانپذیر نمی باشد.",
                        Constants.Notification.MaxCount));

                if (time_diff.TotalHours > 24)
                    sendCount = 1;
            }

            RandomValueGenerator rnd = new RandomValueGenerator();
            var code = rnd.Digit(5);
            if (code[0] == '0') code = rnd.Number(1, 9) + code.Substring(0, 4);

            var hashCode = code.GetHashCode();
            Repository.BulkUpdate(x => username.Equals(x.Username, StringComparison.CurrentCultureIgnoreCase), x => new UserProfile
            {
                PhoneNumberHashedConfirmCode = hashCode,
                PhoneNumberConfirmSendDt = DateTime.Now,
                PhoneNumberConfirmSendCount = sendCount
            });

            var noty = ObjectRegistry.GetObject<INotificationProvider>();
            var result = noty.VerifySms(code, user.PhoneNumber);
            bool success = result.Result == NotificationResultType.Success;
            return new ActionResponse
            {
                Success = success,
                Message = "پیامک تایید ارسال شد."
            };
        }

        public ActionResponse ConfirmPhoneNumberCode(UserProfile profile)
        {
            var hashCode = profile.PhoneNumberConfirmCode.GetHashCode();
            if (profile.PhoneNumberHashedConfirmCode == hashCode)
            {
                var userSrv = ServiceFactory.Create<IAspNetUserService>();
                var user = userSrv.GetCurrent();
                user.PhoneNumberConfirmed = true;
                userSrv.Save(user);
                return new ActionResponse()
                {
                    Success = true,
                    Message = "شماره تماس تایید شد."
                };
            }
            return new ActionResponse("کد وارد شده اشتباه می باشد.");
        }

        [Cacheable(CacheName = CacheConstants.InMemoryCache)]

        public byte[] GetUserProfileAvatar()
        {
            UserProfile prof = GetCurrentProfile();
            return prof.Avatar;

        }
    }

}
