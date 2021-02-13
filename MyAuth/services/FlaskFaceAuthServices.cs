using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAuth.Enums;
using MyAuth.Models.ConfigurationModels;
using MyAuth.Models.Data;
using MyAuth.Models.Interfaces;
using MyAuth.Models.RequestModels;
using MyAuth.Utils.HttpClients;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyAuth.services
{
    public class FlaskFaceAuthServices
    {
        private readonly ILogger<FlaskFaceAuthServices> _logger;
        private readonly FlaskFaceAuthHttpClient _client;
        private readonly MainServicesConfigurations _mainServices;

        public FlaskFaceAuthServices(FlaskFaceAuthHttpClient client, ILogger<FlaskFaceAuthServices> logger, IMainConfigurationModel mainServices)
        {
            _client = client;
            _logger = logger;
            _mainServices = (MainServicesConfigurations)mainServices;
        }



        public async Task<string> IdentifyUser(FlaskFaceAuthIdentifyUserRequestModel request)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, _mainServices.FlaskFaceAuthService.ClientBaseUrl + "/compare");
            var json = JsonConvert.SerializeObject(request);
            message.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var callResult = await _client.Send<HttpResponseData<string, FlaskFaceAuthErrorCodes>>(message);
            return null;
        }


        public async Task RegisterFace()
        {

        }
    }
}
