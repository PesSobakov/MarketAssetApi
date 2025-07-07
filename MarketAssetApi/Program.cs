using MarketAssetApi.Models.Mapper;
using MarketAssetApi.Models.MarketAssetDatabase;
using MarketAssetApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MarketAssetApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //CORS
            string CORSOpenPolicy = "OpenCORSPolicy";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                  name: CORSOpenPolicy,
                  builder =>
                  {
                      builder
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                  });
            });

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<MarketAssetContext>(
                options =>
                {
                    options.UseNpgsql(builder.Configuration["DATABASE_CONNECTION_STRING"], options2 =>
                    {
                        options2.CommandTimeout(300);
                    });
                });

            builder.Services.AddTransient<IMarketAssetService, MarketAssetService>();
            builder.Services.AddTransient<IMarketAssetDatabaseService, MarketAssetDatabaseService>();
            builder.Services.AddTransient<IFintachartsApiService, FintachartsApiService>();

            builder.Services.AddAutoMapper(typeof(MarketAssetProfile).Assembly);

            builder.Services.AddOptions();
            builder.Services.Configure<FintachartsOptions>(builder.Configuration.GetSection("FintachartsOptions"));
            builder.Services.AddHttpClient();
            var app = builder.Build();

            app.UseWebSockets();
            app.UseCors(CORSOpenPolicy);
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
            //app.UseHttpsRedirection();
            app.MapControllers();

            app.Run();
        }
    }
}
