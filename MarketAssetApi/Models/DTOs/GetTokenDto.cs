using System.Text.Json.Serialization;

namespace MarketAssetApi.Models.DTOs
{
    public class GetTokenDto
    {
        [JsonPropertyName("access_token")]
        public required string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public required int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_expires_in")]
        public required int RefreshExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public required string RefreshToken { get; set; }

        [JsonPropertyName("token_type")]
        public required string TokenType { get; set; }

        [JsonPropertyName("not-before-policy")]
        public required int NotBeforePolicy { get; set; }

        [JsonPropertyName("session_state")]
        public required string SessionState { get; set; }

        [JsonPropertyName("scope")]
        public required string Scope { get; set; } 
    }
}
