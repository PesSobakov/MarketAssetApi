using Microsoft.EntityFrameworkCore;
using MarketAssetApi.Models.MarketAssetDatabase;
using MarketAssetApi.Models.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.WebSockets;

namespace MarketAssetApi.Services
{
    public class MarketAssetService : IMarketAssetService
    {
        private readonly IMarketAssetDatabaseService _marketAssetDatabaseService;
        private readonly IFintachartsApiService _fintachartsApiService;
        private readonly IMapper _mapper;

        public MarketAssetService(IMarketAssetDatabaseService marketAssetDatabaseService, IFintachartsApiService fintachartsApiService, IMapper mapper)
        {
            _marketAssetDatabaseService = marketAssetDatabaseService;
            _fintachartsApiService = fintachartsApiService;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<MarketAssetDto>>> GetMarketAssets()
        {
            List<MarketAsset> marketAssets = await _marketAssetDatabaseService.GetMarketAssets();
            List<MarketAssetDto> marketAssetDtos = marketAssets.Select(_mapper.Map<MarketAssetDto>).ToList();
            return new ServiceResponse<List<MarketAssetDto>>()
            {
                Data = marketAssetDtos,
                Status = System.Net.HttpStatusCode.OK
            };
        }

        public async Task<ServiceResponse<List<HistoricalPricesForAssetDto>>> GetHistoricalPrices(List<string> symbols, int interval, string periodicity, DateTime startDate, DateTime endDate)
        {
            if (symbols.Count == 0)
            {
                return new ServiceResponse()
                {
                    Status = System.Net.HttpStatusCode.BadRequest,
                    Error = new ErrorResponse()
                    {
                        Error = new ErrorMessage()
                        {
                            Code = "operation_failed",
                            Message = "At least 1 symbol required"
                        }
                    }
                };
            }

            var marketAssetResponse = await _marketAssetDatabaseService.GetMarketAssetsByName(symbols);
            if (marketAssetResponse.Status != System.Net.HttpStatusCode.OK)
            {
                return new ServiceResponse()
                {
                    Status = marketAssetResponse.Status,
                    Error = marketAssetResponse.Error
                };
            }

            List<MarketAsset> marketAssets = marketAssetResponse.Data!;
            List<HistoricalPricesForAssetDto> historicalPrices = new List<HistoricalPricesForAssetDto>(marketAssets.Count);
            for (int i = 0; i < marketAssets.Count; i++)
            {
                var historicalPricesResponse = await _fintachartsApiService.GetHistoricalPrices(marketAssets[i], interval, periodicity, startDate, endDate);
                if (historicalPricesResponse.Status != System.Net.HttpStatusCode.OK)
                {
                    return new ServiceResponse()
                    {
                        Status = historicalPricesResponse.Status,
                        Error = historicalPricesResponse.Error
                    };
                }
                historicalPrices.Add(new HistoricalPricesForAssetDto()
                {
                    symbol = marketAssets[i].Name,
                    Data = historicalPricesResponse.Data!.Data
                });
            }

            return new ServiceResponse<List<HistoricalPricesForAssetDto>>()
            {
                Data = historicalPrices,
                Status = System.Net.HttpStatusCode.OK
            };
        }

        public async Task<ServiceResponse> GetRealTimePrices(WebSocket webSocket, List<string> symbols)
        {
            if (symbols.Count == 0)
            {
                return new ServiceResponse()
                {
                    Status = System.Net.HttpStatusCode.BadRequest,
                    Error = new ErrorResponse()
                    {
                        Error = new ErrorMessage()
                        {
                            Code = "operation_failed",
                            Message = "At least 1 symbol required"
                        }
                    }
                };
            }

            var marketAssetResponse = await _marketAssetDatabaseService.GetMarketAssetsByName(symbols);
            if (marketAssetResponse.Status != System.Net.HttpStatusCode.OK)
            {
                return new ServiceResponse()
                {
                    Status = marketAssetResponse.Status,
                    Error = marketAssetResponse.Error
                };
            }

            List<MarketAsset> marketAssets = marketAssetResponse.Data!;

            await _fintachartsApiService.GetRealTimePrices(webSocket, marketAssets);
            return new ServiceResponse() { Status = System.Net.HttpStatusCode.OK };
        }

        public async Task<ServiceResponse> Seed()
        {
         ServiceResponse response =  await _marketAssetDatabaseService.Seed();
            return response;
        }
    }
}
