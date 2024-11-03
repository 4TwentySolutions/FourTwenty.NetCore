namespace FourTwenty.Core.ReCaptcha
{
    public record RecaptchaSettings
    {
        /// <summary>
        /// Base URL e.g. https://www.google.com/recaptcha
        /// </summary>
        public string BaseUrl { get; init; } = "https://www.google.com/recaptcha/";

        /// <summary>
        /// Relative URL for verification e.g. api/siteverify
        /// </summary>
        public string VerifyUrl { get; init; } = "api/siteverify";

        /// <summary>
        /// Google Recaptcha Secret Key
        /// </summary>
        public string SecretKey { get; init; } = null!;
        /// <summary>
        /// Google Recaptcha Site Key
        /// </summary>
        public string SiteKey { get; init; } = null!;


    }
}
