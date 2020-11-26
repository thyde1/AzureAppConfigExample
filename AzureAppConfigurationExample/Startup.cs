namespace AzureAppConfigurationExample
{
    using Azure.Identity;
    using AzureAppConfigurationExample.FeatureFilters;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.AzureAppConfiguration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.FeatureManagement;
    using Microsoft.FeatureManagement.FeatureFilters;
    using System;

    public class Startup
    {
        private readonly string Environment;

        public Startup(IWebHostEnvironment env)
        {
            this.Environment = env.EnvironmentName;

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{this.Environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            var appSettings = builder.Build();
            var credentials = new DefaultAzureCredential();
            var appConfigEndpoint = appSettings["ConnectionStrings:AppConfigEndpoint"];
            var region = appSettings["Region"];
            this.Configuration = builder
                .AddAzureAppConfiguration(options => options
                    .Connect(new Uri(appConfigEndpoint), credentials)
                    .UseFeatureFlags()
                    .Select(KeyFilter.Any, LabelFilter.Null)
                    .Select(KeyFilter.Any, this.Environment)
                    .Select(KeyFilter.Any, region)
                    .ConfigureKeyVault(o => o.SetCredential(credentials))
                    .ConfigureRefresh(o => o.SetCacheExpiration(TimeSpan.FromMinutes(1))))
                .Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(this.Configuration);
            services.AddHttpContextAccessor();
            services.AddFeatureManagement()
                .AddFeatureFilter<PercentageFilter>()
                .AddFeatureFilter<SecretFeatureFilter>();
            services.AddControllers();
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
