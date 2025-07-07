using Microsoft.EntityFrameworkCore;

namespace MarketAssetApi.Models.MarketAssetDatabase
{
    public class MarketAssetContext : DbContext
    {
        public MarketAssetContext(DbContextOptions<MarketAssetContext> options) : base(options) { }
        public DbSet<MarketAsset> marketAssets { get; set; }
    }
}
