using MarketAssetApi.Models.DTOs;
using MarketAssetApi.Models.MarketAssetDatabase;
using System.Net.WebSockets;

namespace MarketAssetApi.Services
{
    public interface IMarketAssetService
    {
        public Task<ServiceResponse<List<MarketAssetDto>>> GetMarketAssets();
        public Task<ServiceResponse<List<HistoricalPricesForAssetDto>>> GetHistoricalPrices(List<string> symbols, int interval, string periodicity, DateTime startDate, DateTime endDate);
        public Task<ServiceResponse> GetRealTimePrices(WebSocket webSocket,List<string> symbols);
        public Task<ServiceResponse> Seed();
    }
}
