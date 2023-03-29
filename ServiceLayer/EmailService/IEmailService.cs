using System;
using System.Collections.Generic;
using System.Text;
using Common;
namespace ServiceLayer.EmailService
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
