using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Common
{
    public class SessionTokenDto
    {
        public int Id { get; set; }
        public Guid Name { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual DateTime ExpiresAt { get; set; }
    }
}
