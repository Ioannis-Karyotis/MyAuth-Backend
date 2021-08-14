using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.RequestModels
{
    public class LoginFacialRequestModel
    {
        public string X_seq { get; set; }
        public string FaceDescriptor { get; set; }
    }
}
