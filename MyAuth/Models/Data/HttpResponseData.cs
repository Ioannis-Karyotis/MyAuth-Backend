using MyAuth.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.Data
{
    public class HttpResponseData<T>
        where T : class
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Error { get; set; }

        public HttpResponseData()
        {
        }


    }

}
