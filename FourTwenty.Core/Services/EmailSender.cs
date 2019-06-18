using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using FourTwenty.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace FourTwenty.Core.Services
{
    public class EmailSender : IEmailSender, IEmailService
    {
        // Our private configuration variables
        private readonly string _host;
        private readonly int _port;
        private readonly bool _enableSsl;
        private readonly string _userName;
        private readonly string _password;

        public EmailSender(string host, int port, bool enableSsl, string userName, string password)
        {
            this._host = host;
            this._port = port;
            this._enableSsl = enableSsl;
            this._userName = userName;
            this._password = password;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return SendEmailAsync(email, subject, htmlMessage, null);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage, List<IFormFile> attachments)
        {
            using (var message = new MailMessage(_userName, email, subject, htmlMessage))
            {
                message.IsBodyHtml = true;
                if (attachments != null && attachments.Any())
                {
                    foreach (IFormFile attachment in attachments)
                    {
                        message.Attachments.Add(new Attachment(attachment.OpenReadStream(), attachment.FileName, attachment.ContentType));
                    }
                }
                using (var smtpClient = new SmtpClient(_host, _port)
                {
                    EnableSsl = _enableSsl,
                    Credentials = new NetworkCredential(_userName, _password),
                })
                {
                    await smtpClient.SendMailAsync(message);
                }
            }
        }
    }
}
