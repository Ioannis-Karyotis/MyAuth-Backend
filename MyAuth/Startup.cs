using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyAuth.Data;
using MyAuth.Filters;
using MyAuth.Middleware;
using MyAuth.Models.ConfigurationModels;
using MyAuth.Models.Data;
using MyAuth.Models.Database;
using MyAuth.Models.Interfaces;
using MyAuth.services;
using MyAuth.Services;
using MyAuth.Utils;
using MyAuth.Utils.Extentions;
using MyAuth.Utils.Handlers;
using MyAuth.Utils.HttpClients;

namespace MyAuth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                 options.UseNpgsql(Configuration.GetAppActiveConnectionStringOrDevelopment(),
                 assembly => Configuration.GetActiveDBSchemaOrDevelopment()));

            //Identity Configuration
            //services.AddIdentity<MyAuthUser, IdentityRole<Guid>>(options =>
            //{
            //    options.User.RequireUniqueEmail = false;
            //})
            //.AddEntityFrameworkStores<ApplicationDbContext>()
            //.AddDefaultTokenProviders();

            //services.Configure<IdentityOptions>(options =>
            //{
            //    // Password settings.
            //    options.Password.RequireDigit = true;
            //    options.Password.RequireLowercase = true;
            //    options.Password.RequireNonAlphanumeric = true;
            //    options.Password.RequireUppercase = true;
            //    options.Password.RequiredLength = 6;
            //    options.Password.RequiredUniqueChars = 1;

            //    // Lockout settings.
            //    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            //    options.Lockout.MaxFailedAccessAttempts = 5;
            //    options.Lockout.AllowedForNewUsers = true;

            //    // User settings.
            //    options.User.AllowedUserNameCharacters =
            //    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            //    options.User.RequireUniqueEmail = false;
            //});


            //Add Cors Policy
            services.AddCors(options =>
            {
                options.AddPolicy(
                  "CorsPolicy",
                  builder => builder.WithOrigins("http://localhost:4200")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials()
                  .WithMethods("GET", "POST", "OPTIONS", "DELETE", "PUT"));
            });
            services.AddMemoryCache();

            //Memory Cache Handler Service
            services.AddMemoryHandlerService<MemoryKeys>();

            //HttpClients
            services.AddHttpClient<FlaskFaceAuthHttpClient>();

            //Set Config Data
            services.ConfigBindClasses(Configuration);
            services.AddSingleton<IMainConfigurationModel, MainServicesConfigurations>();

            //HttpContextAccessor
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //Rest Services
            services.AddScoped<AuthServices>();
            services.AddScoped<AccountService>();
            services.AddScoped<ExternalAuthService>();
            services.AddScoped<FlaskFaceAuthServices>(); 
            services.AddScoped<TxtFileValidatorService>(); 
            services.AddScoped<StatusCodesHandler>();



            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddSingleton<RequestValidatorPartsHelper>();


            services.AddAuthentication(IISDefaults.AuthenticationScheme);
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

            app.UseCors("CorsPolicy");

            //API Middleware
            app.UseDevicesMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
