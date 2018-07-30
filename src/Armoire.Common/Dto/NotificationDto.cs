using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Common
{
    public class NotificationDto
    {
        public string RecipientAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public IList<NotificationAttachmentDto> Attachments { get; set; }
    }
}
