using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.ApiResponseModels
{
    public class ExternalSuccessfulLoginRespModel
    {
        public string AuthCode { get; set; }
        public string State { get; set; }
        public string Client_Url { get; set; }
    }
}
