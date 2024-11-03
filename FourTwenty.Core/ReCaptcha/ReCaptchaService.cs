using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace FourTwenty.Core.ReCaptcha
{
    public class ReCaptchaService : IReCaptchaService
    {
        private readonly RecaptchaSettings _recaptchaSettings;
        private readonly HttpClient _client;
        public ReCaptchaService(HttpClient client, IOptions<RecaptchaSettings> settings)
        {
            _recaptchaSettings = settings.Value;
            _client = client;
        }


        public async Task<ReCaptchaResponse?> VerifyCaptcha(string token, string secretKey, string? remoteIp = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_recaptchaSettings.VerifyUrl}?secret={secretKey}&response={token}&remoteip={remoteIp ?? ""}");
            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return new ReCaptchaResponse() { Success = false, ErrorCodes = !string.IsNullOrEmpty(response.ReasonPhrase) ? new[] { response.ReasonPhrase } : null };

            return await response.Content.ReadFromJsonAsync<ReCaptchaResponse>();
        }

    }
}
