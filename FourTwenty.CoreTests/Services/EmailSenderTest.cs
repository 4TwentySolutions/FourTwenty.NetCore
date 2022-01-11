using System.Net.Sockets;
using System.Threading.Tasks;
using FourTwenty.Core.Options;
using FourTwenty.Core.Services;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FourTwenty.CoreTests.Services
{
    [TestClass]
    public class EmailSenderTest
    {

        [TestMethod]
        public async Task SendEmail_BadHost()
        {
            var sender = new EmailSender(new OptionsWrapper<EmailSenderOptions>(new EmailSenderOptions()
            {
                EnableSsl = false,
                FromName = "Test",
                Host = "mail.test.mail",
                Password = "Test",
                Port = 3555,
                UseCertificateValidation = false,
                UserName = "Test"
            }));

            try
            {
                await sender.SendEmailAsync("test@mail.com", "Sub", "Message</br>Test");
            }
            catch (SocketException e)
            {
                Assert.IsNotNull(e);
            }
        }
    }
}
