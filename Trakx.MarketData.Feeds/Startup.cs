using System;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

using Trakx.MarketData.Feeds.Common;
using Trakx.MarketData.Feeds.Common.ApiClients;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(Version, new Info { Title = "Trakx MarketData Api", Version = Version });
                });
            services.AddSingleton<ITrackerComponentProvider, TrackerComponentProvider>();
            services.AddHttpClient(
                ApiConstants.CoinMarketCap.HttpClientName,
                c =>
                    {
                        c.BaseAddress = new Uri(ApiConstants.CoinMarketCap.SandboxEndpoint);
                        c.DefaultRequestHeaders.Add(ApiConstants.CoinMarketCap.ApiKeyHeader, ApiConstants.CoinMarketCap.ApiKeySandbox);
                    });
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
}
