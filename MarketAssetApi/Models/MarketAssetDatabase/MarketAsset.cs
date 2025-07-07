namespace MarketAssetApi.Models.MarketAssetDatabase
{
    public class MarketAsset
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string InstrumentId { get; set; }
    }
}
