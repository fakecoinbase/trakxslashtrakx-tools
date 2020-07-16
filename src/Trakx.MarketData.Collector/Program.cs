using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Trakx.Common.Interfaces;
using Trakx.MarketData.Collector.CryptoCompare;
using Trakx.Persistence;

namespace Trakx.MarketData.Collector
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .ConfigureAppConfiguration(b => b.AddEnvironmentVariables())
                .Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddCryptoCompareClient();

                    services.AddOptions();
                    services.Configure<PriceCacheConfiguration>(
                        hostContext.Configuration.GetSection(nameof(PriceCacheConfiguration)));

                    services.AddSingleton<IPriceCache, PriceCache>();
                    services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

                    services.AddDistributedRedisCache(options =>
                    {
                        options.Configuration = hostContext.Configuration.GetConnectionString("RedisConnection");
                    });
                    
                    services.AddMemoryCache();
                    
                    services.AddDbContext<IndiceRepositoryContext>(options =>
                        options.UseSqlServer(hostContext.Configuration.GetConnectionString("SqlServerConnection")));

                    services.AddScoped<IIndiceDataProvider, IndiceDataProvider>();
                });
    }
}