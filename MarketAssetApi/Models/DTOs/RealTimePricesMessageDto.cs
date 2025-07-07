using System.Text.Json.Serialization;

namespace MarketAssetApi.Models.DTOs
{
    public class RealTimePricesMessageDto
    {
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("instrumentId")]
        public required string InstrumentId { get; set; }

        [JsonPropertyName("provider")]
        public required string Provider { get; set; }

        [JsonPropertyName("subscribe")]
        public required bool Subscribe  { get; set; }

        [JsonPropertyName("kinds")]
        public required List<string> Kinds { get; set; }
    }
}
