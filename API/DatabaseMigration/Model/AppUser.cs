using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

#nullable disable

namespace DatabaseMigration.Model
{
    public partial class AppUser : IdentityUser
    {
        public AppUser()
        {
            EmergencyContacts = new HashSet<EmergencyContact>();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public ICollection<EmergencyContact> EmergencyContacts;
    }
}
