using System.Text.Json.Serialization;

namespace MarketAssetApi.Models.DTOs
{
    public class RealTimePricesUpdateDto
    {
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        [JsonPropertyName("instrumentId")]
        public required string InstrumentId { get; set; }

        [JsonPropertyName("provider")]
        public required string Provider { get; set; }

        [JsonPropertyName("bid")]
        public RealTimePricesBidDto? Bid { get; set; }

        [JsonPropertyName("ask")]
        public RealTimePricesAskDto? Ask { get; set; }

        [JsonPropertyName("last")]
        public RealTimePricesLastDto? Last { get; set; }
    }
}
