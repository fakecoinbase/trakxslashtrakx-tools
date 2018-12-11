using System;

using CryptoCompare;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

using Trakx.MarketData.Feeds.Common.ApiClients;
using Trakx.MarketData.Feeds.Common.Cache;
using Trakx.MarketData.Feeds.Common.Models.Trakx;
using Trakx.MarketData.Feeds.Common.Pricing;
using Trakx.MarketData.Feeds.Common.Trackers;

namespace Trakx.MarketData.Feeds
{
    public class Startup
    {
        private const string Version = "v0.1";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(Version, new Info { Title = "Trakx MarketData Api", Version = Version });
                });

            services.AddSingleton<IPricer, Pricer>();
            services.AddSingleton<IResponseBuilder, ResponseBuilder>();
            services.AddSingleton<ICryptoCompareClient, CryptoCompareClient>();
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddSingleton<ITrakxMemoryCache, TrakxMemoryCache>();
            services.AddSingleton<ITrackerComponentProvider, TrackerComponentProvider>();

            services.AddCoinMarketCapHttpClient();
            services.AddCryptoCompareHttpClient();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"swagger/{Version}/swagger.json", $"Trakx MarketData Api {Version}");
                    c.RoutePrefix = string.Empty;
                });
        }
    }

    public static class IServiceCollectionExtensions
    {
        public static IHttpClientBuilder AddCryptoCompareHttpClient(this IServiceCollection services)
        {
            return services.AddHttpClient(
                ApiConstants.CryptoCompare.HttpClientName,
                c =>
                    {
                        c.BaseAddress = new Uri(ApiConstants.CryptoCompare.SandboxEndpoint);
                        c.DefaultRequestHeaders.Add(ApiConstants.CryptoCompare.ApiKeyHeader, ApiConstants.CryptoCompare.ApiKeyHeaderValue);
                    });
        }

        public static IHttpClientBuilder AddCoinMarketCapHttpClient(this IServiceCollection services)
        {
            return services.AddHttpClient(
                ApiConstants.CoinMarketCap.HttpClientName,
                c =>
                    {
                        c.BaseAddress = new Uri(ApiConstants.CoinMarketCap.SandboxEndpoint);
                        c.DefaultRequestHeaders.Add(ApiConstants.CoinMarketCap.ApiKeyHeader, ApiConstants.CoinMarketCap.ApiKeySandbox);
                    });
        }
    }

}
