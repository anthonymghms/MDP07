using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace DatabaseMigration.Model
{
    public partial class EmergencyContact
    {
        [Key]
        public Guid EmergencyContactId { get; set; }
        public Guid EmergencyContactUserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsDeleted { get; set; }
        public string UserId { get; set; }

        public virtual AppUser User { get; set; }        
    }
}
