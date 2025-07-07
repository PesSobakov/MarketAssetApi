using MarketAssetApi.Models.DTOs;
using MarketAssetApi.Models.MarketAssetDatabase;
using System.Net.WebSockets;

namespace MarketAssetApi.Services
{
    public interface IFintachartsApiService
    {
        public Task<ServiceResponse<HistoricalPricesDto>> GetHistoricalPrices(MarketAsset marketAsset, int interval, string periodicity, DateTime startDate, DateTime endDate);
        public Task<ServiceResponse> GetRealTimePrices(WebSocket webSocket,List<MarketAsset> marketAssets);
    }
}
