using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseMigration.Model
{
    public partial class Log
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Operation { get; set; }
        public string OperationResult { get; set; }
        public DateTime LogDateTime { get; set; }
        public AppUser User { get; set; }
    }
}
