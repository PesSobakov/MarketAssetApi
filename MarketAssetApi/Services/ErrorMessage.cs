using System.Text.Json.Serialization;

namespace MarketAssetApi.Services
{
    public class ErrorMessage
    {
        public required string Code { get; set; }
        public required string Message { get; set; }
    }
}
