using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MyAuth.Models.Interfaces.IEnviromentVarsData;

namespace MyAuth.Models.ConfigurationModels
{
    public class MyAuthAppEnvVars : IAppEnvVars
    {

            public IEnvVar APPSTATERUN { get; set; } = new EnvVar();
            public IEnvVar APPMAINHANDLE { get; set; } = new EnvVar();
            public IEnvVar APPNETENV { get; set; } = new EnvVar();
            public IEnvVar CONTENTROOTLOYAL { get; set; } = new EnvVar();
            public IEnvVar WEBROOTLOYAL { get; set; } = new EnvVar();

    }

    public class EnvVar : IEnvVar
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
