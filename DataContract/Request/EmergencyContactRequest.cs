using System;
using System.Collections.Generic;

namespace DataContract.Request
{
    public class EmergencyContactRequest
    {
        public Guid EmergencyContactId { get; set; }
        public Guid EmergencyContactUserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsDeleted { get; set; }
    }
}
