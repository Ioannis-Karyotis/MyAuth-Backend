using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyAuth.Models.ConfigurationModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Utils.Extentions
{
    public static class ConfigurationExtension
    {
        public static void AddMainConfiguration(this IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.AddJsonFile($"{Path.Combine(EnvVariablesSetter.RealPath, "Configs", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development", "appsettings.json")}");
            configurationBuilder.AddJsonFile($"{Path.Combine(EnvVariablesSetter.RealPath, "Configs", "FlaskFaceAuth_Configuration.json")}");
        }

        public static void ConfigBindClasses(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<AppApiKeys>(config.GetSection("MobilesAppApiKeys"));
            services.Configure<AuthPasswordHash>(config.GetSection("AuthPasswordHashMobiles"));
            services.Configure<FlaskFaceAuthServiceConfig>(config.GetSection("FlaskFaceAuthServiceConfig"));
        }
    }
}
