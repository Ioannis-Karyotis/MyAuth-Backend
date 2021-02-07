using MyAuth.Utils.LibReflect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.Interfaces
{
    public class IEnviromentVarsData
    {
        public interface IAppEnvVars
        {
            public IEnvVar APPMAINHANDLE { get; set; }
            public IEnvVar APPSTATERUN { get; set; }
            public Dictionary<string, IEnvVar> GetPropertiesUsingReflection() => ReflectAction.DicOfPropAndValsFromInstance<IEnvVar>(this);
        }

        public interface IEnvVar
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}
