using System;
using System.Collections.Generic;

#nullable disable

namespace DatabaseMigration.Model
{
    public partial class User
    {
        public User()
        {
            EmergencyContacts = new HashSet<UserEmergencyContact>();
        }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime CreationDate { get; set; }

        public ICollection<UserEmergencyContact> EmergencyContacts;
    }
}
