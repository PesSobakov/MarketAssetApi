
using MarketAssetApi.Models.MarketAssetDatabase;

namespace MarketAssetApi.Services
{
    public interface IMarketAssetDatabaseService
    {
        public Task<List<MarketAsset>> GetMarketAssets();
        public Task<ServiceResponse<List<MarketAsset>>> GetMarketAssetsByName(List<string> symbols);
        public Task<ServiceResponse> Seed();
    }
}
