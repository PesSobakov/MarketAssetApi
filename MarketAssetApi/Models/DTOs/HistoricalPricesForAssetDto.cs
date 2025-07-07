using System.Text.Json.Serialization;

namespace MarketAssetApi.Models.DTOs
{
    public class HistoricalPricesForAssetDto
    {
        [JsonPropertyName("symbol")]
        public required string symbol { get; set; }
        [JsonPropertyName("data")]
        public required List<HistoricalPriceItemDto> Data { get; set; }
    }
}
