using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyAuth.Utils.LibReflect
{
    public class ReflectAction
    {
        public static PropertyInfo[] ListOfPropertiesFromInstance(Type AType)
        {
            return AType.GetProperties(BindingFlags.Public);
        }

        public static PropertyInfo[] ListOfPropertiesFromInstance(object InstanceOfAType)
        {
            if (InstanceOfAType == null) return null;
            Type TheType = InstanceOfAType.GetType();
            return TheType.GetProperties(BindingFlags.Public);
        }

        public static Dictionary<string, T> DicOfPropAndValsFromInstance<T>(object InstanceOfAType)
        {
            if (InstanceOfAType == null)
                return null;
            Type TheType = InstanceOfAType.GetType();
            PropertyInfo[] Properties = TheType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            Dictionary<string, T> PropertiesMap = new Dictionary<string, T>();
            foreach (PropertyInfo Prop in Properties)
            {
                PropertiesMap.Add(Prop.Name, (T)Prop.GetValue(InstanceOfAType));
            }
            return PropertiesMap;
        }
    }
}
