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
                .Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddCryptoCompareClient();

                    services.AddSingleton<IPriceCache, PriceCache>();

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