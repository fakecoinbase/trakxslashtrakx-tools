using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using Trakx.Common.Interfaces;
using Trakx.MarketData.Collector.CryptoCompare;
using Trakx.Persistence;

namespace Trakx.MarketData.Collector
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddCryptoCompareClient();
                    services.AddDistributedRedisCache(options =>
                    {
                        options.Configuration = hostContext.Configuration.GetConnectionString("RedisConnection");
                        options.ConfigurationOptions.ReconnectRetryPolicy = new ExponentialRetry(100, 120_000);
                    });
                    services.AddDbContext<IndexRepositoryContext>(options =>
                        options.UseSqlServer(hostContext.Configuration.GetConnectionString("SqlServerConnection")));
                    services.AddSingleton<IIndexDataProvider, IndexDataProvider>();
                });
    }
}