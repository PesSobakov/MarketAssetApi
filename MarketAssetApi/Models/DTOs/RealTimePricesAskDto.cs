using System.Text.Json.Serialization;

namespace MarketAssetApi.Models.DTOs
{
    public class RealTimePricesAskDto
    {
        [JsonPropertyName("timestamp")]
        public required DateTimeOffset Timestamp { get; set; }

        [JsonPropertyName("price")]
        public required decimal Price { get; set; }

        [JsonPropertyName("volume")]
        public required long Volume { get; set; }
    }
}