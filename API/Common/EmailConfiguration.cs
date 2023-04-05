using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class EmailConfiguration
    {
        public string From { get; set; } = null!;
        public string SmtpServer { get; set; } = null!;
        public int Port { get; set; }
        public string ApiKey { get; set; }
        public string ApiKeySecret { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
