using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FourTwenty.Core.Interfaces;
using FourTwenty.Core.Options;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FourTwenty.Core.Services
{
    public class EmailSender : IEmailService, IEmailSender
    {

        private readonly EmailSenderOptions _emailSenderOptions;

        public EmailSender(string host, int port, bool enableSsl, string userName, string password, bool useCertificateValidation = false, string fromName = "My Application") =>
            _emailSenderOptions = new EmailSenderOptions(host, port, enableSsl, userName, password,
                useCertificateValidation, fromName);


        public EmailSender(IOptions<EmailSenderOptions> emailOptions)
        {
            _emailSenderOptions = emailOptions.Value;
        }


        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return SendEmailAsync(email, subject, htmlMessage, null);
        }

        public virtual async Task SendEmailAsync(string email, string subject, string htmlMessage, List<IFormFile> attachments)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSenderOptions.FromName, _emailSenderOptions.UserName));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = subject;

            var body = new TextPart("html")
            {
                Text = htmlMessage
            };

            var multipart = new Multipart("mixed") { body };

            foreach (var attachment in attachments.Select(file => new MimePart(file.ContentType)
            {
                Content = new MimeContent(file.OpenReadStream()),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(file.FileName)
            }))
            {
                multipart.Add(attachment);
            }

            message.Body = multipart;
            using var client = new SmtpClient();
            if (_emailSenderOptions.UseCertificateValidation)
                client.ServerCertificateValidationCallback = ValidateCertificate;

            await client.ConnectAsync(_emailSenderOptions.Host, _emailSenderOptions.Port, _emailSenderOptions.EnableSsl);

            await client.AuthenticateAsync(_emailSenderOptions.UserName, _emailSenderOptions.Password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        protected virtual bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            const SslPolicyErrors ignoredErrors =
                SslPolicyErrors.RemoteCertificateChainErrors |  // self-signed
                SslPolicyErrors.RemoteCertificateNameMismatch;  // name mismatch

            // If the certificate is a valid, signed certificate, return true.
            if ((sslPolicyErrors & ~ignoredErrors) == SslPolicyErrors.None)
                return true;

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) == 0) return false;

            // In all other cases, return false.
            if (chain?.ChainStatus == null) return true;
            foreach (X509ChainStatus status in chain.ChainStatus)
            {
                if (certificate.Subject == certificate.Issuer &&
                    (status.Status == X509ChainStatusFlags.UntrustedRoot))
                {
                    // Self-signed certificates with an untrusted root are valid. 
                    continue;
                }

                if (status.Status != X509ChainStatusFlags.NoError)
                {
                    // If there are any other errors in the certificate chain, the certificate is invalid,
                    // so the method returns false.
                    return false;
                }
            }

            // When processing reaches this line, the only errors in the certificate chain are 
            // untrusted root errors for self-signed certificates. These certificates are valid
            // for default Exchange server installations, so return true.
            return true;
        }
    }
}
