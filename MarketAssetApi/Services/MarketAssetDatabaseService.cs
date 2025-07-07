using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;
using System.Net;
using System.Linq;
using System.Globalization;
using MarketAssetApi.Models.MarketAssetDatabase;

namespace MarketAssetApi.Services
{
    public class MarketAssetDatabaseService : IMarketAssetDatabaseService
    {
        private readonly MarketAssetContext _marketAssetContext;
        public MarketAssetDatabaseService(MarketAssetContext context)
        {
            _marketAssetContext = context;
        }

        public async Task<List<MarketAsset>> GetMarketAssets()
        {
            List<MarketAsset> marketAssets = await _marketAssetContext.marketAssets
                .ToListAsync();

            return marketAssets;
        }

        public async Task<ServiceResponse<List<MarketAsset>>> GetMarketAssetsByName(List<string> symbols)
        {
            List<MarketAsset> marketAssets = await _marketAssetContext.marketAssets
                .Where(asset => symbols.Contains(asset.Name))
                .ToListAsync();

            if (marketAssets.Count != symbols.Count)
            {
                List<string> missingSymbols = symbols.Where(symbol => !marketAssets.Select(asset => asset.Name).Contains(symbol)).ToList();
                return new ServiceResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    Error = new ErrorResponse()
                    {
                        Error = new ErrorMessage()
                        {
                            Code = "operation_failed",
                            Message = "symbols not found: " + string.Join(", ", missingSymbols.Select(symbol => "'" + symbol + "'"))
                        }
                    }
                };
            }

            return new ServiceResponse<List<MarketAsset>>()
            {
                Status = HttpStatusCode.OK,
                Data = marketAssets
            };
        }

        public async Task<ServiceResponse> Seed()
        {
            await _marketAssetContext.Database.EnsureCreatedAsync();
            _marketAssetContext.RemoveRange(_marketAssetContext.marketAssets);
            List<MarketAsset> marketAssets = [
                new MarketAsset(){
                    Name = "AUD/CAD",
                    InstrumentId = "ad9e5345-4c3b-41fc-9437-1d253f62db52"
                },
                new MarketAsset(){
                    Name = "AUD/CHF",
                    InstrumentId = "e394fa5b-bba1-45fe-b7b9-7e7c0124425b"
                },
                new MarketAsset(){
                    Name = "AUD/JPY",
                    InstrumentId = "7fd43865-35af-4de6-9c25-9a6684292ce3"
                },
                new MarketAsset(){
                    Name = "AUD/NZD",
                    InstrumentId = "6f772323-e675-4a0e-89e6-ad8673274936"
                },
                new MarketAsset(){
                    Name = "AUD/USD",
                    InstrumentId = "db1246ba-3bb4-4945-8012-381754baab0e"
                },
                new MarketAsset(){
                    Name = "CAD/CHF",
                    InstrumentId = "63d4a5b6-351f-41a0-969f-6e5bfcd32fcd"
                },
                new MarketAsset(){
                    Name = "CAD/JPY",
                    InstrumentId = "054dc5aa-7d4e-45b5-abea-11cb24823ce4"
                },
                new MarketAsset(){
                    Name = "CHF/JPY",
                    InstrumentId = "ba827038-6bff-4fd6-b9fc-40e68cf1701f"
                },
                new MarketAsset(){
                    Name = "EUR/AUD",
                    InstrumentId = "ec46af30-414b-439c-bda2-35506405a0fd"
                },
                new MarketAsset(){
                    Name ="EUR/CAD",
                    InstrumentId = "ec15a527-5a7c-47ac-9357-24a051efaa1e"
                },
                new MarketAsset(){
                    Name = "EUR/CHF",
                    InstrumentId = "cc3c06aa-db08-4f70-8d53-212b968235b3"
                },
                new MarketAsset(){
                    Name = "EUR/GBP",
                    InstrumentId = "6aa6b522-4de6-454e-b5b9-c08a67089206"
                },
                new MarketAsset(){
                    Name = "EUR/JPY",
                    InstrumentId = "fd7d35bc-271e-4000-b18d-257b4d505ee5"
                },
                new MarketAsset(){
                    Name = "EUR/USD",
                    InstrumentId = "ebefe2c7-5ac9-43bb-a8b7-4a97bf2c2576"
                },
                new MarketAsset(){
                    Name = "GBP/AUD",
                    InstrumentId = "a03f13c0-ae75-4ab6-8642-a32bd29197c3"
                },
                new MarketAsset(){
                    Name = "GBP/CHF",
                    InstrumentId = "44677e52-88f5-44f1-b28d-67c9550be39b"
                },
                new MarketAsset(){
                    Name = "GBP/CHF",
                    InstrumentId = "ffb77545-8df1-4aa4-b089-8450ef2d5127"
                },
                new MarketAsset(){
                    Name = "GBP/USD",
                    InstrumentId = "5071e03c-80d3-4b28-870d-28533306c8c6"
                },
                new MarketAsset(){
                    Name = "USD/CAD",
                    InstrumentId = "fc34928e-cc93-4009-bc4e-578e20a7d77b"
                },
                new MarketAsset(){
                    Name = "USD/CHF",
                    InstrumentId = "7d6cc901-3fcd-4542-96b4-4223a861ff8a"
                }];
            _marketAssetContext.AddRange(marketAssets);
            await _marketAssetContext.SaveChangesAsync();
            return new ServiceResponse() { Status = HttpStatusCode.OK };
        }


    }
}
