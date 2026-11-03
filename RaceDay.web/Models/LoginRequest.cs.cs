
using System.Text.Json.Serialization;

namespace RaceDay.Web.Models
{
    public class LoginRequest
    {
        [JsonPropertyName("Email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("Password")]
        public string Password { get; set; } = string.Empty;
    }
}

