using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FourTwenty.Core.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using MimeKit;

namespace FourTwenty.Core.Services
{
    public class EmailSender : IEmailService
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
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("MACS Application", _userName));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = subject;

            var body = new TextPart("plain")
            {
                Text = htmlMessage,
            };
            
            var multipart = new Multipart("mixed");
            foreach (var file in attachments)
            {
                // create an image attachment for the file located at path
                var attachment = new MimePart("text/plain")
                {
                    Content = new MimeContent(file.OpenReadStream()),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName(file.FileName)
                };
                // now create the multipart/mixed container to hold the message text and the
                // image attachment
                multipart.Add(body);
                multipart.Add(attachment);
            }
           
            message.Body = multipart;
            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                //client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(_host, _port, _enableSsl);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(_userName, _password);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
