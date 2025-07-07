using System.Text.Json.Serialization;

namespace MarketAssetApi.Models.DTOs
{
    public class HistoricalPricesDto
    {
        [JsonPropertyName("data")]
        public required List<HistoricalPriceItemDto> Data { get; set; }

    }
}
