using System.Data;
using System.Data.SqlClient;
using GoogleClient;
using IdentityService.Infrastructure;
using IdentityService.Infrastructure.Configuration;
using IdentityService.Persistence.Implementations;
using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Settings = new Settings(configuration);
        }

        public IConfiguration Configuration { get; }
        public Settings Settings { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton(Settings);
            services.AddSingleton<IDbConnection>(sp => new SqlConnection(Settings.DbConnectionString));
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton(new GoogleClient.GoogleClient(new GoogleSettings
            {
                ServiceUrl = Settings.GoogleServiceUrl
            }));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseExceptionMiddleware();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
