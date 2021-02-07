using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Utils.Extentions
{
    public static class StartingPathsDeclerationsExtension
    {
        public static IHostBuilder SetAppContentRootByENV(this IHostBuilder hostBuilder)
        {
            var rec = EnvVariablesRetriever.GetAppContentRootPath(); //For VM MUST be configured
            if (!String.IsNullOrEmpty(rec))
            {
                hostBuilder.UseContentRoot(rec);
            }
            return hostBuilder;
        }

        public static IWebHostBuilder SetAppWebRootByENV(this IWebHostBuilder hostWebBuilder)
        {
            var recWeb = EnvVariablesRetriever.GetAppWebRootPath(); //For VM MUST be configured
            if (!String.IsNullOrEmpty(recWeb))
            {
                hostWebBuilder.UseWebRoot(recWeb);
            }
            return hostWebBuilder;
        }
    }
}
