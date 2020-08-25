using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Services.Notifications
{
    public class KavenegarNotificationProvider : INotificationProvider
    {
        public NotificationResponse SendEmail(string subject, string body, string receiver, bool html)
        {
            MailMessage mail = new MailMessage();
            using (SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["mail:smtp"]))
            {

                try
                {
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["mail:from"]);
                    mail.To.Add(receiver);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = html;

                    SmtpServer.Port = int.Parse(ConfigurationManager.AppSettings["mail:port"]);
                    SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["mail:username"], ConfigurationManager.AppSettings["mail:password"]);
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);
                }
                catch (Exception exception)
                {
                    return new NotificationResponse()
                    {
                        Result = NotificationResultType.Error,
                        Message = exception.Message
                    }; ;
                }
                return new NotificationResponse()
                {
                    Result = NotificationResultType.Success
                };
            }
        }

        public NotificationResponse SendSms(string text, string[] receivers)
        {
            string apikey = ConfigurationManager.AppSettings[Constants.Kavenegar.ApiKey];
            var api = new Kavenegar.KavenegarApi(apikey);
            var result = api.Send(ConfigurationManager.AppSettings["kavenegar-sender"], receivers.ToList(), text);
            return new NotificationResponse()
            {
                Result = result.All(x => x.Status == 200) ? NotificationResultType.Success : NotificationResultType.Error
            };
        }

        public NotificationResponse VerifyEmail(string subject, string body, string receiver, bool html)
        {
            return SendEmail(subject, body, receiver, html);
        }

        public NotificationResponse VerifySms( string token, string receiver)
        {
            string apikey = ConfigurationManager.AppSettings[Constants.Kavenegar.ApiKey];
            var api = new Kavenegar.KavenegarApi(apikey);
            var result = api.VerifyLookup(receiver, token, Constants.Kavenegar.svipVerifyPhone);
            return new NotificationResponse()
            {
                Message = result.Message,
                Result = NotificationResultType.Success
            };
        }
    }
}
