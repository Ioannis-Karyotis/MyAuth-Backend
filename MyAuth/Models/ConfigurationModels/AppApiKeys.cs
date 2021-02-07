using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.ConfigurationModels
{
    public class AppApiKeys
    {
        public List<string> AvailableKeys { get; set; } = new List<string>();
    }
}
