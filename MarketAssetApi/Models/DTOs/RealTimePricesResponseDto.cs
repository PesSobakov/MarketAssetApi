using System.Text.Json.Serialization;

namespace MarketAssetApi.Models.DTOs
{
    public class RealTimePricesResponseDto
    {
        [JsonPropertyName("symbol")]
        public required string Symbol { get; set; }

        [JsonPropertyName("bid")]
        public RealTimePricesBidDto? Bid { get; set; }

        [JsonPropertyName("ask")]
        public RealTimePricesAskDto? Ask { get; set; }

        [JsonPropertyName("last")]
        public RealTimePricesLastDto? Last { get; set; }
    }
}
