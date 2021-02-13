using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Utils.Extentions
{
    public static class EnumExtentions
    {
        public static string GetName<T>(this T val) where T : Enum => Enum.GetName(typeof(T), val);
        public static T GetValueByName<T>(this string val) where T : Enum => (T)Enum.Parse(typeof(T), val, true);
    }
}
