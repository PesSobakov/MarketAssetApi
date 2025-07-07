using System.Text.Json.Serialization;

namespace MarketAssetApi.Models.DTOs
{
    public class HistoricalPriceItemDto
    {
        [JsonPropertyName("t")]
        public required DateTimeOffset T { get; set; }

        [JsonPropertyName("o")]
        public required decimal Open { get; set; }

        [JsonPropertyName("h")]
        public required decimal High { get; set; }

        [JsonPropertyName("l")]
        public required decimal Low { get; set; }

        [JsonPropertyName("c")]
        public required decimal Close { get; set; }

        [JsonPropertyName("v")]
        public required long Volume { get; set; }
    }
}
