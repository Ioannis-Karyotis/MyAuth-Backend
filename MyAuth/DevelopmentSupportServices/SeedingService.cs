using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.DevelopmentSupportServices
{
    public static class SeedingService
    {
        public static IHost PrepareHostAndSeed(this IHostBuilder hostBuilder)
        {
            var host = hostBuilder.Build();

            using (var scope = host.Services.CreateScope())
            {
                var servicesScope = scope.ServiceProvider.CreateScope();
                var services = servicesScope.ServiceProvider;
                try
                {
                    //services.SeedRoles();
                    //services.SeedSuperAdmin();
                    //services.MigrateTheSeedsFromDevDB();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
            return host;
        }
    }
}
