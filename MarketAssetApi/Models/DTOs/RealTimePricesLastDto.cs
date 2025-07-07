using System.Text.Json.Serialization;

namespace MarketAssetApi.Models.DTOs
{
    public class RealTimePricesLastDto
    {
        [JsonPropertyName("timestamp")]
        public required DateTimeOffset Timestamp { get; set; }

        [JsonPropertyName("price")]
        public required decimal Price { get; set; }

        [JsonPropertyName("volume")]
        public required long Volume { get; set; }

        [JsonPropertyName("change")]
        public required decimal Change { get; set; }

        [JsonPropertyName("changePct")]
        public required decimal ChangePct { get; set; }
    }
}
