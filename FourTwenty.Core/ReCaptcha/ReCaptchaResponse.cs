using Newtonsoft.Json;

namespace FourTwenty.Core.ReCaptcha
{
    public class ReCaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("challenge_ts")]
        public string ChallengeTs { get; set; }
        [JsonProperty("hostname")]
        public string Hostname { get; set; }
        [JsonProperty("error-codes")]
        public string[] ErrorCodes { get; set; }
        [JsonProperty("score")]
        public float Score { get; set; }
        [JsonProperty("action")]
        public string Action { get; set; }
    }
}
