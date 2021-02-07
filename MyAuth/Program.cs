using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyAuth.DevelopmentSupportServices;
using MyAuth.Utils;
using MyAuth.Utils.Extentions;

namespace MyAuth
{
    public class Program
    {
        public static string RealPath { get; set; } = "";
        public static void Main(string[] args)
        {
            EnvVariablesSetter.StartReadSet();
            CreateHostBuilder(args).PrepareHostAndSeed().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .SetAppContentRootByENV()
            .ConfigureAppConfiguration((hostContext, config) => config.AddMainConfiguration())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.SetAppWebRootByENV();
                    webBuilder.UseStartup<Startup>();
                });
    }
}
