using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContract.Request
{
    public class ForgotPasswordRequest
    {
        public string Username { get; set; }
        public string NewPassword { get; set; }
    }
}
