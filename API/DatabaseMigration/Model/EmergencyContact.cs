using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace DatabaseMigration.Model
{
    public partial class EmergencyContact
    {
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid EmergencyContactUserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsDeleted { get; set; }

        public virtual AppUser User { get; set; }        
    }
}
