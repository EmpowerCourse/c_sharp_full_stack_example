using System;

namespace Armoire.Models
{
    [Serializable]
    public class ErrorVM
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string Message { get; set; }
    }
}