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
        public string AlertType { get; set; }
        public float AlertVolume { get; set; }
        public bool LocationSharing { get; set; }
        public bool DarkMode { get; set; }

        public virtual AppUser User { get; set; }
    }
}
