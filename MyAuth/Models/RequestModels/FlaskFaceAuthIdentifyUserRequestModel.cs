using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.RequestModels
{
    public class FlaskFaceAuthIdentifyUserRequestModel
    {
        public string Base64Img { get; set; }
        public string Data { get; set; }
    }
}
