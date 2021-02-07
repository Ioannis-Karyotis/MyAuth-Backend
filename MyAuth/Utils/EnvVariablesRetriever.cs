using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Utils
{
    public static class EnvVariablesRetriever
    {
        public static string GetRealBasePath() => Environment.GetEnvironmentVariable("ASPENVMAINHANDLE");
        public static string GetActiveDBSchema() => $"Schema{Environment.GetEnvironmentVariable("ASPENVMAINACT")}";
        public static string GetActiveDBSchemaOrDevelopment(this IConfiguration config) => (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPENVMAINACT"))) ? "DBAlmaPetLoyaltyFinal" : config.GetConnectionString($"Schema{Environment.GetEnvironmentVariable("ASPENVMAINACT")}");

        public static string GetAppContentRootPath() => Environment.GetEnvironmentVariable("CONTENTROOTLOYAL");
        public static string GetAppWebRootPath() => Environment.GetEnvironmentVariable("WEBROOTLOYAL");
        public static string GetAppActiveConnectionString() => Environment.GetEnvironmentVariable("ASPENVMAINACT");
        //Created For Migration creation and update database connection string selection its a result of env value ASPENVMAINACT
        public static string GetAppActiveConnectionStringOrDevelopment(this IConfiguration config) => (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPENVMAINACT"))) ?
                                                                                                    "Host=localhost;Database=MyAuth;Username=postgres;Password=Margoleta16!" :
                                                                                                     config.GetConnectionString(EnvVariablesRetriever.GetAppActiveConnectionString());

    }
}
