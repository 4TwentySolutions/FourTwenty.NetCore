namespace FourTwenty.Core.Options
{
    public class EmailSenderOptions
    {
        public EmailSenderOptions(string host, int port, bool enableSsl, string userName, string password, bool useCertificateValidation, string fromName)
        {
            Host = host;
            Port = port;
            EnableSsl = enableSsl;
            UserName = userName;
            Password = password;
            UseCertificateValidation = useCertificateValidation;
            FromName = fromName;
        }

        public EmailSenderOptions()
        {
            
        }

        public string Host { get; set; }

        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UseCertificateValidation { get; set; }
        public string FromName { get; set; }

    }
}
