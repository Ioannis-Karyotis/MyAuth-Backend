using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.RequestModels
{
    public class ChangePassRequestModel
    {
        public string OldPass { get; set; }
        public string NewPass { get; set; }
        public string NewPassVal { get; set; }
    }
}
