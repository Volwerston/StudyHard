using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IdentityService.Persistence.Implementations;
using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using StudyHard.Persistence.Implementations;
using StudyHard.Persistence.Interfaces;
using StudyHard.Service.Implementations;
using StudyHard.Service.Interfaces;

namespace StudyHard
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Settings = new Settings(Configuration);
        }

        public IConfiguration Configuration { get; }
        public Settings Settings;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;

                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.SymmetricSigninKey)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            var connectionString = Configuration.GetConnectionString("StudyHardDatabase");
            var dbConnection = new SqlConnection(connectionString);
            services.AddSingleton<IDbConnection>(sp => new SqlConnection(connectionString));
            services.AddControllersWithViews();
            services.AddSingleton<IUserRepository, UserRepository>(provider => new UserRepository(dbConnection, connectionString));
            services.AddSingleton(Settings);
            services.AddTransient<ICourseRepository, CourseRepository>(provider => new CourseRepository(connectionString));
            services.AddTransient<ITutorRepository, TutorRepository>(provider => new TutorRepository(connectionString));
            services.AddTransient<IChatRepository, ChatRepository>(provider => new ChatRepository(connectionString));
            
            services.AddTransient<IChatService, ChatService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.Use(async (context, next) =>
            {
                var token = context.Request.Cookies[".AspNetCore.Application.Id"];
                if (!string.IsNullOrEmpty(token))
                    context.Request.Headers.Add("Authorization", "Bearer " + token);

                await next();
            });

            app.UseStatusCodePages(context => {
                var request = context.HttpContext.Request;
                var response = context.HttpContext.Response;

                if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    response.Redirect("/Login");
                }

                return Task.CompletedTask;
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
