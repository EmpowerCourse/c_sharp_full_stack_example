using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;

namespace Armoire.Common
{
    public interface INotificationService
    {
        bool EmailAddressIsValid(string emailAddress);
        bool PhoneNumberIsValid(string phone);
        //void EnqueueEmail(NotificationDto notification);
        void SendEmailNow(NotificationDto notification);
        //void SendSMSNow(NotificationDto notification, string phoneNo, string customerFullName);
        //bool IsValidPhoneNumber(string phoneNo);
        //NotificationSendResultDto ProcessEmailNotifications();
        //bool ProcessEmailNotificationsWithSuccessOrFailure();
        //IList<NotificationDto> GetPendingNotifications();
        //NotificationDto Get(int notificationId);
        //IList<NotificationSearchResultDto> Find(NotificationSearchCriteria criteria);
        //void SendCustomEmail(IList<string> recipientEmailList, UserDto sentByUser, string subject, string body);
    }
}
