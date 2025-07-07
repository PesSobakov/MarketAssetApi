using System.Text.Json.Serialization;

namespace MarketAssetApi.Services
{
    public class ErrorResponse
    {
        public required ErrorMessage Error { get; set; }
    }
}
