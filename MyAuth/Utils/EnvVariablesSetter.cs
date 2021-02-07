using MyAuth.Models.ConfigurationModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static MyAuth.Models.Interfaces.IEnviromentVarsData;

namespace MyAuth.Utils
{
    public class EnvVariablesSetter
    {
        public static string RealPath = Program.RealPath;
        public static string logo = @"

            ███╗░░░███╗██╗░░░██╗░█████╗░██╗░░░██╗████████╗██╗░░██╗  ██████╗░░█████╗░░█████╗░██╗░░██╗███████╗███╗░░██╗██████╗░
            ████╗░████║╚██╗░██╔╝██╔══██╗██║░░░██║╚══██╔══╝██║░░██║  ██╔══██╗██╔══██╗██╔══██╗██║░██╔╝██╔════╝████╗░██║██╔══██╗
            ██╔████╔██║░╚████╔╝░███████║██║░░░██║░░░██║░░░███████║  ██████╦╝███████║██║░░╚═╝█████═╝░█████╗░░██╔██╗██║██║░░██║
            ██║╚██╔╝██║░░╚██╔╝░░██╔══██║██║░░░██║░░░██║░░░██╔══██║  ██╔══██╗██╔══██║██║░░██╗██╔═██╗░██╔══╝░░██║╚████║██║░░██║
            ██║░╚═╝░██║░░░██║░░░██║░░██║╚██████╔╝░░░██║░░░██║░░██║  ██████╦╝██║░░██║╚█████╔╝██║░╚██╗███████╗██║░╚███║██████╔╝
            ╚═╝░░░░░╚═╝░░░╚═╝░░░╚═╝░░╚═╝░╚═════╝░░░░╚═╝░░░╚═╝░░╚═╝  ╚═════╝░╚═╝░░╚═╝░╚════╝░╚═╝░░╚═╝╚══════╝╚═╝░░╚══╝╚═════╝░
         ";
        public static void StartReadSet()
        {
            var data = GetAppEnvVars();
            Console.WriteLine($"Web App Started");
            Console.WriteLine(logo);
            if (data != null)
            {
                var envVars = data.GetPropertiesUsingReflection();
                foreach (var item in envVars)
                {
                    if (item.Value != null && !String.IsNullOrEmpty(item.Value.Value))
                    {
                        Environment.SetEnvironmentVariable(item.Value.Key, item.Value.Value);
                        Console.WriteLine($"Gonfig ENV Item Set: {item.Key}");
                    }
                    else if (item.Value != null && !String.IsNullOrEmpty(item.Value.Key))
                    {
                        Environment.SetEnvironmentVariable(item.Value.Key, null);
                        Console.WriteLine($"Gonfig ENV Item Reset: {item.Key}");
                    }
                }
            }
        }

        private static IAppEnvVars GetAppEnvVars()
        {
            var envPath = EnvVariablesRetriever.GetRealBasePath(); //For VM Must be configured
            Program.RealPath = (String.IsNullOrEmpty(envPath)) ? Directory.GetCurrentDirectory() : envPath;
            var path = Path.Combine(RealPath, "Configs", "Enviroment", "appenviroment.json");
            if (File.Exists(path))
            {
                var doc = File.ReadAllText(path);

                try
                {
                    var objectVars = JsonConvert.DeserializeObject<MyAuthAppEnvVars>(doc);
                    return objectVars;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}, {e.StackTrace}");
                    goto FailureMain;
                }
            }
        FailureMain: return null;
        }
    }
}
