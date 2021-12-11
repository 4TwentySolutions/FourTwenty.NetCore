using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace FourTwenty.Core.ReCaptcha
{
    public class ReCaptchaService : IReCaptchaService
    {
        private static readonly string ClientName = $"{nameof(ClientName)}_{nameof(ReCaptchaService)}";
        private const string ReCaptchaUrl = "https://www.google.com/recaptcha/api/siteverify";
        private readonly RecaptchaSettings _recaptchaSettings;
        private readonly IHttpClientFactory _clientFactory;
        public ReCaptchaService(IOptions<RecaptchaSettings> settings, IHttpClientFactory httpClientFactory)
        {
            _recaptchaSettings = settings.Value;
            _clientFactory = httpClientFactory;
        }

        public ReCaptchaService(RecaptchaSettings settings, IHttpClientFactory httpClientFactory)
        {
            _recaptchaSettings = settings;
            _clientFactory = httpClientFactory;
        }

        public async Task<ReCaptchaResponse> VerifyCaptcha(string token, string remoteIp = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post,
                $"{ReCaptchaUrl}?secret={_recaptchaSettings.SecretKey}&response={token}&remoteip={remoteIp ?? ""}");
            var response = await _clientFactory.CreateClient(ClientName).SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return new ReCaptchaResponse() {Success = false, ErrorCodes = new[] {response.ReasonPhrase}};

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ReCaptchaResponse>(content);

        }


    }
}
