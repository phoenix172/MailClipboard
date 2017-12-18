using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace MailClipboard
{
    class MailSender
    {
        private readonly AppConfiguration _config;

        public MailSender(AppConfiguration config, SecureString password)
        {
            _config = config;
            Password = password;
        }

        public void SendEmail(string selectedText)
        {
            var fromAddress = new MailAddress(_config.SenderEmail, _config.SenderName);
            var toAddress = new MailAddress(_config.RecipientEmail, _config.RecipientName);
            string subject = selectedText.Trim();

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, Password)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = _config.EmailBody
            })
            {
                smtp.Send(message);
            }
        }

        public SecureString Password { get; set; }
    }
}
