using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.RequestModels
{
    public class AddAppRequestModel
    {
        public string AppName{ get; set; }
        public string BaseUrl{ get; set; }
        public string RedirectUrl{ get; set; }
    }
}
