using MyAuth.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Utils
{
    public class InternalDataTransfer<T>
    {
        public InternalDataStatuses Status { get; set; }
        public T Data { get; set; }
    }
}


