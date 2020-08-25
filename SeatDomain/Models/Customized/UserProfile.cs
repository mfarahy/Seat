using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Models
{
    public partial class UserProfile
    {

        #region Email related properties

        public bool EmailConfirmed { get; set; }
        public string Email { get; set; }
        public string EmailConfirmCode { get; set; }
        public bool EmailExceedWaitTimeLimit
        {
            get
            {
                if (EmailConfirmSendDt == null) return false;
                var time_diff = DateTime.Now - EmailConfirmSendDt.Value;
                return time_diff < TimeSpan.FromMinutes(Constants.Notification.WaitTime);
            }
        }
        public bool EmailExceedSendCountLimit
        {
            get
            {
                if (EmailConfirmSendDt == null) return false;
                var time_diff = DateTime.Now - EmailConfirmSendDt.Value;
                return EmailConfirmSendCount > Constants.Notification.MaxCount && time_diff.TotalHours < 24;
            }
        }
        public TimeSpan? EmailTimeDiff
        {
            get
            {
                return DateTime.Now - EmailConfirmSendDt;
            }
        }
        public DateTime? EmailWaitTime
        {
            get
            {
                if (EmailTimeDiff == null) return null;
                return DateTime.Now + TimeSpan.FromMinutes(Constants.Notification.WaitTime) - EmailTimeDiff;
            }
        }
        public bool CanSendEmailConfirmCode
        {
            get
            {
                return !EmailExceedWaitTimeLimit && !EmailExceedSendCountLimit;
            }
        }
        public bool CanAcceptEmailConfirmCode
        {
            get
            {
                return EmailConfirmSendDt != null && !EmailConfirmed;
            }
        }
        #endregion


        #region phone number related properties

        public bool PhoneNumberConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberConfirmCode { get; set; }
        public bool PhoneExceedWaitTimeLimit
        {
            get
            {
                if (PhoneNumberConfirmSendDt == null) return false;
                var time_diff = DateTime.Now - PhoneNumberConfirmSendDt.Value;
                return time_diff < TimeSpan.FromMinutes(Constants.Notification.WaitTime);
            }
        }

        public bool PhoneExceedSendCountLimit
        {
            get
            {
                if (PhoneNumberConfirmSendDt == null) return false;
                var time_diff = DateTime.Now - PhoneNumberConfirmSendDt.Value;
                return PhoneNumberConfirmSendCount > Constants.Notification.MaxCount && time_diff.TotalHours < 24;
            }
        }

        public TimeSpan? PhoneTimeDiff
        {
            get
            {
                return DateTime.Now - PhoneNumberConfirmSendDt;
            }
        }

        public DateTime? PhoneWaitTime
        {
            get
            {
                if (PhoneTimeDiff == null) return null;
                return DateTime.Now + TimeSpan.FromMinutes(Constants.Notification.WaitTime) - PhoneTimeDiff;
            }
        }

        public bool CanSendPhoneNumberConfirmCode
        {
            get
            {
                return !PhoneExceedWaitTimeLimit && !PhoneExceedSendCountLimit;
            }
        }

        public bool CanAcceptPhoneNumberConfirmCode
        {
            get
            {
                return PhoneNumberConfirmSendDt != null && !PhoneNumberConfirmed;
            }
        } 
        #endregion
    }
}
