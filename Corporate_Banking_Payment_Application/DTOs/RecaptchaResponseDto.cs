using System.Text.Json.Serialization;

namespace Corporate_Banking_Payment_Application.DTOs
{
    public class RecaptchaResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error-codes")]
        public IEnumerable<string>? ErrorCodes { get; set; }
    }
}