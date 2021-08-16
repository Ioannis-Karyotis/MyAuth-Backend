using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.ApiResponseModels
{
    public class AppDetailsRespModel
    {
        public string AppName{ get; set; }
        public string BaseUrl { get; set; }
        public string RedirectUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

    }
}
