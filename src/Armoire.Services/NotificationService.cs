using Armoire.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace Armoire.Services
{
    public class NotificationService : INotificationService
    {
        private static readonly Regex emailHostRegex = new Regex(@"(\w+)\.(\w+)");
        private const string TESTING_SUBJECT_PREFIX_TEMPLATE = "**APP TESTING** (will go to [{0}] with subject [{1}]): ";
        private readonly SmtpClient _smtpClient;
        private readonly ISettingsService _settingService;

        public NotificationService(SmtpClient smtpClient, ISettingsService settingService)
        {
            _smtpClient = smtpClient;
            _settingService = settingService;
        }

        private static bool emailAddressIsValid(string emailAddress)
        {
            if (String.IsNullOrEmpty(emailAddress) || !emailAddress.Contains("@")) return false;
            try
            {
                var mail = new MailAddress(emailAddress);
                return emailHostRegex.IsMatch(mail.Host);
            }
            catch
            {
                return false;
            }
        }

        public bool PhoneNumberIsValid(string phone)
        {
            return Regex.IsMatch(phone, AppConstants.PHONE_NUMBER_REGEX_PATTERN);
        }

        public bool EmailAddressIsValid(string emailAddress)
        {
            return emailAddressIsValid(emailAddress);
        }

        private MailMessage createMailMessage(string recipientEmail, string subject, string body)
        {
            string resolvedSubject;
            string resolvedRecipientEmail;
            string testRecipient = (string)_settingService.GetValue("Email:TestEmailRecipient");
            if (String.IsNullOrWhiteSpace(testRecipient))
            {
                resolvedSubject = subject;
                resolvedRecipientEmail = recipientEmail;
            }
            else
            {
                resolvedSubject = String.Format(TESTING_SUBJECT_PREFIX_TEMPLATE, recipientEmail, subject);
                resolvedRecipientEmail = testRecipient;
            }
            body = executeNotificationStringReplacements(body, (string)_settingService.GetValue("ApplicationURL"), true);
            MailMessage email = new MailMessage(
                new MailAddress((string)_settingService.GetValue("Email:MailFromAddress"), (string)_settingService.GetValue("Email:MailFromName")),
                new MailAddress(resolvedRecipientEmail))
            {
                Subject = resolvedSubject,
                Body = body,
                IsBodyHtml = true
            };
            addAlternateHTMLViewWithImageEmbedding(email, body);
            return email;
        }

        public void SendEmailNow(NotificationDto notification)
        {
            if (!emailAddressIsValid(notification.RecipientAddress))
            {
                string errMsg = String.Format("[{0}] is not a valid email address; the related message was NOT sent", notification.RecipientAddress);
                throw new ApplicationException(errMsg);
            }
            var mailMessage = createMailMessage(notification.RecipientAddress, notification.Subject, notification.Body);
            if (notification.Attachments != null && notification.Attachments.Any())
            {
                foreach (var att in notification.Attachments)
                {
                    var stream = new MemoryStream(att.Content);
                    mailMessage.Attachments.Add(new Attachment(stream, att.Name, att.MimeType));
                }
            }
            _smtpClient.Send(mailMessage);
        }

        private static string executeNotificationStringReplacements(string body, string appURL, bool html)
        {
            if (html)
            {
                return body
                    .Replace("[LINK]", String.Format("<a href='{0}'>{1}</a>", appURL, AppConstants.APP_NAME))
                    ;
            }
            return body
                .Replace("[LINK]", appURL)
                ;
        }

        private void addAlternateHTMLViewWithImageEmbedding(MailMessage mailMessage, string body)
        {
            // embedded logo; refactor if there are going to be multiple such items
            const string embeddedItemKey = "APPLogo";
            if (body.Contains(String.Format("[{0}]", embeddedItemKey)))
            {
                body = body.Replace(String.Format("[{0}]", embeddedItemKey),
                                    String.Format("<img src='cid:{0}'>", embeddedItemKey));
                AlternateView avHtml = AlternateView.CreateAlternateViewFromString(body, null, System.Net.Mime.MediaTypeNames.Text.Html);
                avHtml.LinkedResources.Add(
                    new LinkedResource((string)_settingService.GetValue("LogoPath"), (string)_settingService.GetValue("LogoContentType")) // System.Net.Mime.MediaTypeNames.Image.Gif
                    {
                        ContentId = embeddedItemKey
                    });
                mailMessage.AlternateViews.Add(avHtml);
            }
        }
    }
}
