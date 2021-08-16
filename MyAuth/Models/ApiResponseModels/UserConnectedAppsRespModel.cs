using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.ApiResponseModels
{
    public class AppConnectedApp
    {
        public Guid Id { get; set; }
        public string AppName { get; set; }
        public string AppUrl { get; set; }
        public DateTime DateConnected { get; set; }
    }

    public class UserConnectedAppsRespModel
    {
        public List<AppConnectedApp> ConnectedApps { get; set; }
        public UserConnectedAppsRespModel()
        {
            ConnectedApps = new List<AppConnectedApp>();
        }
    }
}
