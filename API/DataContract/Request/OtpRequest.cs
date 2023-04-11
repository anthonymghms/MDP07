using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContract.Request
{
    public class OtpRequest
    {
        public string username { get; set; }
        public string otp { get; set; }
    }
}
