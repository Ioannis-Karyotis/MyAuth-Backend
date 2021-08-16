using MyAuth.Enums;
using MyAuth.Utils.HttpClients.ClientsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MyAuth.Utils.Extentions;


namespace MyAuth.Models.Data
{
    public class HttpResponseData<T, TErrorCode>
        where TErrorCode : Enum
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public ErrorData<TErrorCode> Error { get; set; }
        public HttpData CallDescription { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public HttpResponseMessage ResponseObject { get; set; } = null;

        public HttpResponseData()
        {
        }

        public HttpResponseData(T data, HttpMethod method = null, Uri uriCalled = null)
        {
            Success = true;
            Data = data;
            if (method != null && uriCalled != null)
            {
                CallDescription = new HttpData
                {
                    Method = method,
                    RequestUrl = uriCalled
                };
            }
        }

        public HttpResponseData(TErrorCode errorCode, HttpMethod method = null, Uri uriCalled = null, string errorDescription = null)
        {
            Success = false;
            Error = new ErrorData<TErrorCode>()
            {
                ErrorCode = errorCode,
                Description = errorDescription == null ? errorCode.GetName<TErrorCode>() : errorDescription
            };
            if (method != null && uriCalled != null)
            {
                CallDescription = new HttpData
                {
                    Method = method,
                    RequestUrl = uriCalled
                };
            }
        }
    }


    public class HttpData
    {
        public HttpMethod Method { get; set; }
        public Uri RequestUrl { get; set; }
    }



}
