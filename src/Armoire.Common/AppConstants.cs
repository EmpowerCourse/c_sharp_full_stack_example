using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Common
{
    public class AppConstants
    {
        public const int PLACEHOLDER_ROW_ID = 0;
        public const int UNPAGED_PAGE_SIZE_IN_ROWS = 10000;
        public const int COOKIE_EXPIRATION_DAYS = 10;
        public const string USER_COOKIE_KEY = "APPU";
        public const string SESSION_COOKIE_KEY = "APS";
        public const int SESSION_TIMEOUT_MINUTES = 240;
        public const int SESSION_REVALIDATION_MINUTES = 10;
        public const string DATETIME_FORMAT = "MM/dd/yyyy hh:mm tt";
        public const string DATE_FORMAT = "MM/dd/yyyy";
        public const string APP_NAME = "Armoire";
        public const string PHONE_NUMBER_REGEX_PATTERN = @"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]‌​)\s*)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-‌​9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})$";
        public const string AUTH_TYPE = APP_NAME + " Authentication";
        public const string DESCRIPTOR_Long = "LongDescription";
        public const string DESCRIPTOR_Short = "ShortDescription";
        public const string CLAIM_TYPE_USER_ID = "UserId";
        public const string CLAIM_TYPE_USER_FULL_NAME = "FullName";
        //public const string CACHEKEY_REDIRECT = "REDIRECT_TO";
        public const string ERR_ACCESS_DENIED = "Access is denied!";
        //public const string CACHEKEY_PREFIX_USER = "USER:";
        //public const string CACHEKEY_PREFIX_DASHBOARD = "DASHBOARD:";
        //public const string CACHEKEY_PREFIX_PRINCIPAL = "PRINCIPAL:";
        //public const string CACHEKEY_PREFIX_ERRORMESSAGE = "ERROR_MESSAGE";
        public const string CACHEKEY_PREFIX_LOGONMESSAGE = "LOGON_MESSAGE";
        public const string CACHEKEY_LOGON_REDIRECT_USER = "USER_ON_AUTH";
        public const string CACHEKEY_PREFIX_FORGOTPASSWORD = "FORGOT_PWD:";
        //public const string CACHEKEY_DTO_VALIDATION_REDIRECT = "CACHEKEY_DTO_VALIDATION_REDIRECT";
        public const string PASSWORD_COMPLEXITY_REGEX = ".{6,}";
        public const string PASSWORD_COMPLEXITY_REGEX_EXPLANATION = "at least 6 characters";
        public static class MimeTypes
        {
            public const string TEXT_PLAIN = "text/plain";
            public const string JSON = "application/json";
            public const string HTML = "text/html";
            public const string OCTET_STREAM = "application/octet-stream";
            public const string PDF = "application/pdf";
            public const string JPG = "image/jpeg";
            public const string GIF = "image/gif";
            public const string PNG = "image/png";
            public const string CSV = "text/csv";
            public const string XLS = "application/vnd.ms-excel";
            public const string ZIP = "application/zip";
            public const string XLSX = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }
    }
}
