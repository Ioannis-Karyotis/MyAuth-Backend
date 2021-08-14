using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.ApiResponseModels
{
    public class SuccessfulLoginFirstStepRespModel
    {
        public Guid Id { get; set; }
        public string X_seq{ get; set; }
    }
}
