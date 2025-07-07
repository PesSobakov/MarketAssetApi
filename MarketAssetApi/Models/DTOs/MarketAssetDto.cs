using System.Text.Json.Serialization;

namespace MarketAssetApi.Models.DTOs
{
    public class MarketAssetDto
    {
        [JsonPropertyName("symbol")]
        public required string Name { get; set; }
    }
}
