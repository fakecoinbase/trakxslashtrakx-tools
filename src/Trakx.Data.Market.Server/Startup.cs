using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Trakx.Data.Market.Common.Pricing;
using Trakx.Data.Market.Common.Sources.CoinGecko;
using Trakx.Data.Market.Common.Sources.CryptoCompare;
using Trakx.Data.Market.Server.Areas.Identity;
using Trakx.Data.Market.Server.Data;
using Trakx.Data.Market.Server.Hubs;
using Trakx.Data.Models.Index;
using Trakx.Data.Models.Initialisation;

namespace Trakx.Data.Market.Server
{
    public class Startup
    {
        private const string ApiName = "Trakx Market Data Api";
        private const string Version = "v0.1";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ContainerConnection")));
            
            services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddDbContext<IndexRepositoryContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ContainerConnection")));

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSignalR().AddMessagePackProtocol();

            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(Version, new OpenApiInfo { Title = ApiName, Version = Version });
            });

            services.AddScoped<IIndexDefinitionProvider, IndexDefinitionProvider>();
            services.AddScoped<NavHub>();
            services.AddPricing();
            services.AddCoinGeckoClient();
            services.AddCryptoCompareClient();

            services.AddMemoryCache();

            // DB Creation and Seeding
            services.AddTransient<IDatabaseInitialiser, DatabaseInitialiser>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            SeedDatabase(app);
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{Version}/swagger.json", ApiName);
                c.InjectJavascript($"public/index.js");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NavHub>("/hubs/nav");
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private static void SeedDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var databaseInitializer = serviceScope.ServiceProvider.GetService<IDatabaseInitialiser>();
            databaseInitializer.SeedDatabase().Wait();
        }
    }
}
