using Exir.Framework.Common;
using System;
using System.Collections.Generic;

namespace SeatDomain.Services.Notifications
{
    public interface INotificationProvider
    {
         NotificationResponse SendEmail(string subject, string body, string receiver, bool html);

         NotificationResponse SendSms(string text, string[] receivers);

         NotificationResponse VerifyEmail(string subject, string body, string receiver, bool html);

         NotificationResponse VerifySms(string token, string receiver);
    }

    public enum NotificationResultType
    {
        Success = 0,
        Error = 1
    }
    public class NotificationResponse
    {
        public NotificationResultType Result { get; set; }
        public string Message { get; set; }
    }


}
