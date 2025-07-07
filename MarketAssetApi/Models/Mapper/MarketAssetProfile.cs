using AutoMapper;
using MarketAssetApi.Models.DTOs;
using MarketAssetApi.Models.MarketAssetDatabase;

namespace MarketAssetApi.Models.Mapper
{
    public class MarketAssetProfile : Profile
    {
        public MarketAssetProfile()
        {
            CreateMap<MarketAsset, MarketAssetDto>();
        }
    }
}
