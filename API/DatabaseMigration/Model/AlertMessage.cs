using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseMigration.Model
{
    public partial class AlertMessage
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string ToUserId { get; set; }
        public string MessageText { get; set; }
        public DateTime MessageDateTime { get; set; }
        public bool IsRead { get; set; }
        public virtual AppUser User { get; set; }
    }
}
