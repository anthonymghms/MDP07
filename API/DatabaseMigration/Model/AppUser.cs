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
            UserAlerts = new HashSet<UserAlert>();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public ICollection<EmergencyContact> EmergencyContacts;
        public ICollection<UserAlert> UserAlerts;
        public UserConfig UserConfig;
    }
}
