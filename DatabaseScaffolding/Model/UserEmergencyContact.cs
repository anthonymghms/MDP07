using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseMigration.Model
{
    public class UserEmergencyContact
    {
        public Guid UserId { get; set; }
        public Guid EmergencyContactId { get; set; }
        public User User { get; set; }
        public EmergencyContact EmergencyContact { get; set; }
    }
}
