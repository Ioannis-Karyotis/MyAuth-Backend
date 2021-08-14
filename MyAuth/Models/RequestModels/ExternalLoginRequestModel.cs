using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.RequestModels
{
    public class ExternalLoginRequestModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Response_type { get; set; }
        public string Client_id{ get; set; }
        public string Redirect_uri { get; set; }
        public string Scope { get; set; }
        public string State { get; set; }
    }
}
