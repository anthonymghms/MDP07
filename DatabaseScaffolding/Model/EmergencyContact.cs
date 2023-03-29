using System;
using System.Collections.Generic;

#nullable disable

namespace DatabaseMigration.Model
{
    public partial class EmergencyContact
    {
        public EmergencyContact()
        {
            Users = new HashSet<UserEmergencyContact>();
        }
        public Guid EmergencyContactId { get; set; }
        public Guid EmergencyContactUserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<UserEmergencyContact> Users { get; set; }        
    }
}
