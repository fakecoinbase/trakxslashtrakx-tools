using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Trakx.IndiceManager.Server.Data;
using Trakx.IndiceManager.Server.Managers;
using Trakx.Persistence;
using Trakx.Persistence.Initialisation;
using Microsoft.OpenApi.Models;
using Trakx.Common.Ethereum;
using Trakx.Common.Sources.CoinGecko;


namespace Trakx.IndiceManager.Server
{
    public class Startup
    {
        private const string ApiName = "Trakx Indice Manager Api";
        private const string ApiVersion = "v0.1";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           services.AddDbContext<IndiceRepositoryContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SqlServerConnection")));

            services.AddControllers();
            services.AddScoped<IComponentInformationRetriever, ComponentInformationRetriever>();

            services.AddMappings();
            // DB Creation and Seeding
            services.AddTransient<IDatabaseInitialiser, DatabaseInitialiser>();
            services.AddCoinGeckoClient();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(ApiVersion, new OpenApiInfo { Title = ApiName, Version = ApiVersion });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.AddEthereumInteraction();
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{ApiVersion}/swagger.json", ApiName);
                c.InjectJavascript($"public/index.js");
            });

            app.UseAuthorization();

            SeedDatabase(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Welcome to the Indice Manager API");
                });
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
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
