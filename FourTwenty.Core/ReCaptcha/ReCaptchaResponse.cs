using System.Text.Json.Serialization;

namespace FourTwenty.Core.ReCaptcha
{
    public class ReCaptchaResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("challenge_ts")]
        public string ChallengeTs { get; set; }
        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }
        [JsonPropertyName("error-codes")]
        public string[] ErrorCodes { get; set; }
        [JsonPropertyName("score")]
        public float Score { get; set; }
        [JsonPropertyName("action")]
        public string Action { get; set; }
    }
}
