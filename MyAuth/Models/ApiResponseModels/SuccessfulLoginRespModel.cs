using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.ApiResponseModels
{
    public class SuccessfulLoginRespModel
    {
        public string AuthToken { get; set; }
        public string X_Seq { get; set; }
        public string Email{ get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateExpired { get; set; }
    }
}
