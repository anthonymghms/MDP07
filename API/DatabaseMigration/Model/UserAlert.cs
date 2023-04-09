using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseMigration.Model
{
    public partial class UserAlert
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string AlertSeverity { get; set; }
        public string AlertMessage { get; set; }
        public DateTime AlertDateTime { get; set; }

        public virtual AppUser User { get; set; }
    }
}
