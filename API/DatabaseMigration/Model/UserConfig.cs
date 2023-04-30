using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseMigration.Model
{
    public partial class UserConfig
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool LocationSharing { get; set; }
        public bool DarkMode { get; set; }
        public bool NotificationsEnabled { get; set; }
        public bool TwoFactorAuthEnabled { get; set; }
        public float AlertVolume { get; set; }
        public string AlertType { get; set; }
        public string AlertLevel { get; set; }
        public string IpCamAddress { get; set; }
        public string IpEspAddress { get; set; }
        public virtual AppUser User { get; set; }
    }
}
