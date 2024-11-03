using System.Text.Json.Serialization;

namespace FourTwenty.Core.ReCaptcha
{
    public record ReCaptchaResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; }
        [JsonPropertyName("challenge_ts")]
        public string? ChallengeTs { get; init; }
        [JsonPropertyName("hostname")]
        public string? Hostname { get; init; }
        [JsonPropertyName("error-codes")]
        public string[]? ErrorCodes { get; init; }
        [JsonPropertyName("score")]
        public float Score { get; init; }
        [JsonPropertyName("action")]
        public string? Action { get; init; }
    }
}
