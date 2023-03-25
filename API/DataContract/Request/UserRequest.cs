using System;
using System.Collections.Generic;
using DatabaseScaffolding.Model;

namespace DataContract.Request
{
    public class UserRequest
    {
        public UserRequest()
        {
            Devices = new HashSet<Device>();
            EmergencyContacts = new HashSet<EmergencyContact>();
        }

        public Guid UserId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDisabled { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string Email { get; set; }

        public virtual ICollection<Device> Devices { get; set; }
        public virtual ICollection<EmergencyContact> EmergencyContacts { get; set; }
    }
}
