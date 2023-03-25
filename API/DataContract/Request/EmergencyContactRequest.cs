using System;
using DatabaseScaffolding.Model;

namespace DataContract.Request
{
    public class EmergencyContactRequest
    {
        public Guid EmergencyContactId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsUser { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime RegistrationDate { get; set; }

        public virtual User User { get; set; }
    }
}
