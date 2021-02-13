using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Utils.HttpClients.ClientsData
{
    public class ErrorData<TErrorCode> where TErrorCode : Enum
    {
        public TErrorCode ErrorCode { get; set; }
        public string Description { get; set; }
        public string StackTrace { get; set; }
    }
}
