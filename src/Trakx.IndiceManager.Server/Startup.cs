using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Trakx.IndiceManager.Server.Data;
using Trakx.Persistence;
using Trakx.Persistence.Initialisation;
using Microsoft.OpenApi.Models;
using Trakx.Coinbase.Custody.Client;
using Trakx.Common.Ethereum;
using Trakx.Common.Interfaces;
using Trakx.Common.Sources.CoinGecko;
using Trakx.Common.Utils;
using Trakx.IndiceManager.Server.Middlewares;


namespace Trakx.IndiceManager.Server
{
    public class Startup
    {
        private const string ApiName = "Trakx Indice Manager Api";
        private const string ApiVersion = "v0.1";
        private const string ApiDescription = "Trakx' API used to create and administrate indices.";
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
            services.AddAllManagerForControllers();
            services.AddDatabaseFunctions();

            services.AddMappings();
            // DB Creation and Seeding
            services.AddTransient<IDatabaseInitialiser, DatabaseInitialiser>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(ApiVersion, new OpenApiInfo { Title = ApiName, Version = ApiVersion, Description = ApiDescription });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            //this is only to allow auto generating client on build in Trakx.IndiceManager.Client
            services.AddOpenApiDocument(settings =>
            {
                settings.PostProcess = document =>
                {
                    document.Info.Version = ApiVersion;
                    document.Info.Title = ApiName;
                    document.Info.Description = ApiDescription;
                };
            });

            services.AddScoped<IDepositorAddressRetriever, DepositorAddressRetriever>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            services.AddEthereumInteraction(Environment.GetEnvironmentVariable("INFURA_API_KEY"));
            services.AddMemoryCache();
            services.AddCoinGeckoClient();
            services.AddSingleton<ICoinbaseTransactionListener, CoinbaseTransactionListener>();
            services.AddSingleton<IBalanceUpdater, BalanceUpdater>();
            services.AddCoinbaseLibrary(Environment.GetEnvironmentVariable("COINBASE_API_KEY"),
                Environment.GetEnvironmentVariable("COINBASE_PASSPHRASE_KEY"));
            services.AddHostedService<BalanceUpdaterService>();

            services.AddAuthorization();
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = MockJwtTokens.SecurityKey,
                        ValidateLifetime = true,
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
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
            SwaggerBuilderExtensions.UseSwagger(app);
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{ApiVersion}/swagger.json", ApiName);
                c.InjectJavascript($"public/index.js");
            });
            app.UseMiddleware<GetLabelsFromBearer>();
            app.UseAuthentication();
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
