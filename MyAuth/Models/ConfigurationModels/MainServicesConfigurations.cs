using Microsoft.Extensions.Options;
using MyAuth.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.ConfigurationModels
{
    public class MainServicesConfigurations : IMainConfigurationModel
    {
        public FlaskFaceAuthServiceConfig FlaskFaceAuthService { get; set; }

        public MainServicesConfigurations(IOptions<FlaskFaceAuthServiceConfig> optionsFlaskFaceAuth)
        {
            FlaskFaceAuthService = optionsFlaskFaceAuth.Value;
        }
    }
}
