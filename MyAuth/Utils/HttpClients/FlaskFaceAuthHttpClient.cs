using Microsoft.AspNetCore.Http;
using MyAuth.Enums;
using MyAuth.Models.Data;
using MyAuth.Models.FlaskFaceAuth.ResponseModels;
using MyAuth.Utils.HttpClients.ClientsData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MyAuth.Utils.HttpClients
{
    public interface IFresPrototypeHttpClient
    {
        Task<HttpResponseData<T, FlaskFaceAuthErrorCodes>> Send<T>(HttpRequestMessage request, CancellationToken cancellationToken = default) where T : class;
    }
    public class FlaskFaceAuthHttpClient
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _accessor;



        public FlaskFaceAuthHttpClient(HttpClient client, IHttpContextAccessor accessor)
        {
            _client = client;
            _accessor = accessor;
        }

        public async Task<HttpResponseData<T, FlaskFaceAuthErrorCodes>> Send<T>(HttpRequestMessage request, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var httpResponseMessage = await _client.SendAsync(request, cancellationToken);
                var contentString = await httpResponseMessage.Content.ReadAsStringAsync();
                var result = new HttpResponseData<T, FlaskFaceAuthErrorCodes>();
                result.CallDescription = new HttpData
                {
                    Method = request.Method,
                    RequestUrl = request.RequestUri
                };
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    result.Success = true;
                    result.Data = JsonConvert.DeserializeObject<T>(contentString);
                }
                else
                {
                    result.Success = false;
                    result.Error = new ErrorData<FlaskFaceAuthErrorCodes> { ErrorCode = FlaskFaceAuthErrorCodes.GenericError, Description = contentString };
                }

                return result;
            }
            catch (Exception e)
            {
                return new HttpResponseData<T, FlaskFaceAuthErrorCodes>
                {
                    Error = new ErrorData<FlaskFaceAuthErrorCodes>
                    {
                        ErrorCode = FlaskFaceAuthErrorCodes.GenericError,
                        Description = e.Message,
                        StackTrace = e.StackTrace
                    }
                };
            }
        }

    }
}
