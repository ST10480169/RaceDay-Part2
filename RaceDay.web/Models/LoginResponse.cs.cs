
using System.Text.Json.Serialization;

namespace RaceDay.Web.Models
{
    public class LoginResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;
    }
}
