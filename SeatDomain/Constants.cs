using System.Data.SqlTypes;

namespace SeatDomain
{
    public static class Constants
    {
        public static class Notification
        {
            public const int WaitTime = 1;
            public const int MaxCount = 5;
        }
        public static class Messages
        {
            public const string email_confirm_subject = "email_confirm_subject";
            public const string email_confirm_body = "email_confirm_body";
            public const string sms_confirm_text = "sms_confirm_text";

            public static string email_confirmed { get; set; }
            public static string email_not_confirmed { get; set; }
        }
        public static class SystemMessage
        {
            public static class Media
            {
                public const string Sms = "sms";
                public const string Email = "email";
            }
            public static class Cultures
            {
                public const string fa = "fa-ir";
                public const string en = "en-us";
            }
        }
        public const long BigDealsMinMoney = 1_000_000_000;

        public static class Kavenegar
        {
            public const string ApiKey = "kavenegar-apikey";
            public const string svipVerifyPhone = "svipVerifyPhone";
        }

        public static class KnownUsers
        {
            public const string admin = "admin";
        }
        public static class KnownRoles
        {
            public const string admin = "admin";
        }
        public static class JobLogStates
        {
            public const short Success = 1;
            public const short Error = 1;
            public const short Executing = 1;
        }
        public static class QueueStates
        {
            public const string BuyQueue = "صف خرید";
            public const string SellQueue = "صف فروش";
            public const string LightSellQueue = "صف فروش سبک";
            public const string LightBuyQueue = "صف خرید سبک";
            public const string HeavyBuyQueue = "صف خرید سنگین";
            public const string HeavySellQueue = "صف فروش سنگین";
            public const string Balanced = "متوازن";
        }
        public static class BigDeals
        {
            public static class DealTypes
            {
                public const byte Buy = 1;
                public const byte Sell = 2;
            }
            public static class TraderTypes
            {
                public const byte Individual = 1;
                public const byte Legal = 2;
            }
        }
        public static class InstrumentTextTypes
        {
            public const string HousingFacilities = "تسهیلات مسکن";
            public const string Saham = "سهام";
            public const string PayeFarabourse = "پایه فرابورس";
            public const string HaghTaghaddom = "حق تقدم";
            public const string OraghMosharekat = "اوراق مشارکت";
            public const string Ati = "آتی";
            public const string Sandoogh = "صندق";
            public const string EkhtiarForoush = "اختیار فروش";
            public const string Kala = "کالا";
            public const string unknown = "نامشخص";
        }
        public enum InstrumentTypes
        {
            HousingFacilities,
            Saham,
            PayeFarabourse,
            HaghTaghaddom,
            OraghMosharekat,
            Ati,
            Sandoogh,
            EkhtiarForoush,
            Kala,
            unknown
        }
        public static class KnownInstruments
        {
            public const long Overall_Index = 32097828799138957;
            public const long OTC_Overall_Index = 43685683301327984;
            public const long TotalEquel_Weithed = 67130298613737946;
            public const long Industy_Index = 43754960038275285;
        }
        public static class CompanyCodes
        {
            public const string IDXS = "IDXS";
            public const string MSKZ = "MSKZ";
            public const string MELZ = "MELZ";
        }
        public static class Cache
        {
            public const string ConfigCacheKey = "mmp-config-last-update";
            public const string OnlineUserCacheKeyPrefix = "oum:online-users-list";
        }

        public static class FileExtentions
        {
            public const string JPG = "jpg";
            public const string PDF = "pdf";
            public const string Word = "docx";
            public const string PNG = "png";
            public const string JPEG = "jpeg";
        }
        public static class BackstageJobs
        {
            public static class Queue
            {
                public const string Default = "default";
                public const string Notification = "notification";
            }
            public static class Priorities
            {
                public const short High = 1;
                public const short Normal = 2;
                public const short Low = 3;
            }
            public static class States
            {
                public const short New = 1;
                public const short Fetched = 6;
                public const short Processing = 2;
                public const short Done = 3;
                public const short Error = 4;
                public const short Removed = 5;
            }
        }
        public static class KnownOperations
        {
            public const string HlpInfrmtn_A = "HlpInfrmtn.A";
        }
        public static class News
        {
            public const string TopNewsCategoryCode = "TopNews";
            public const string SlideNewsCategoryCode = "SlideNews";
            public const string ApplicationsTopNewsCategoryCode = "ApplicationsTopNews";
            public const string ApplicationsSlideNewsCategoryCode = "ApplicationsSlideNews";
        }

        public static class RuleSets
        {
            public const string GridView = "GridView";
            public const string WithDetails = "WithDetails";
            public const string WithMaster = "WithMaster";
            public const string Restricted = "Restricted";
            public const string WithoutFiles = "WithoutFiles";
            public const string Tree_Copy = "treecopy";
            public const string Tree_Move = "treemove";
        }

        public static class Resources
        {
            public static class Sections
            {
                public const string ErrorMessages = "ErrorMessages";
                public const string Messages = "Messages";
                public const string Glossary = "Glossary";
            }
            public static class Messages
            {
                public const string feedback_okay_message = "feedback_okay_message";
                public const string feedback_has_no_email_and_mobile = "feedback_has_no_email_and_mobile";
                public const string feed_back_response_sms_template = "feed_back_response_sms_template";
                public const string feed_back_response_body_template = "feed_back_response_body_template";
                public const string feed_back_response_subject = "feed_back_response_subject";
            }
        }

        public static class FeedbackStates
        {
            public const byte New = 0;
            public const byte Inestigated = 1;
            public const byte Removed = 2;
            public const byte Responsed = 3;
            public const byte InvalidResponse = 4;
        }
        public static class ExtentionTypes
        {
            public const byte JPG = 0;
            public const byte PDF = 1;
            public const byte Word = 2;
            public const byte PNG = 3;
            public const byte JPEG = 4;
            public const byte TEXT = 5;
            public const byte Unknown = 250;
        }
    }
}