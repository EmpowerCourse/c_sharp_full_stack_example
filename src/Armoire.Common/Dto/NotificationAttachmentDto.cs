using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Common
{
    public class NotificationAttachmentDto
    {
        public string Name { get; set; }
        public string MimeType { get; set; }
        public byte[] Content { get; set; }
    }
}
