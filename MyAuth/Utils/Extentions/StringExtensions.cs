using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Utils.Extentions
{
    public static class StringExtensions
    {
        public static string EncodeToBase64(this string data) => System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data));
        public static string DecodeFromBase64(this string data) => System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(data));

        public static T TransformTo<T>(this string data) => JsonConvert.DeserializeObject<T>(data);

        public static string TransformToJson(this object data) => JsonConvert.SerializeObject(data);


        public static bool AnyIsNull(this string[] vals)
        {
            foreach (var item in vals)
            {
                if (item == null)
                    return true;
            }
            return false;
        }

        public static bool CheckAllCasesIsNotNull(this string val) => !string.IsNullOrEmpty(val) && !string.IsNullOrWhiteSpace(val);

        public static bool ContainsChars(this string value, bool mustContainAll = false, params string[] valsRule)
        {
            if (mustContainAll)
            {
                return value.All(d => value.Any(f => f == d));
            }
            else
            {
                return value.Any(d => value.Any(f => f == d));
            }
        }

        public static bool ValidatePasswordRules(this string value, bool mustHaveSpecialChars = true, bool mustContainNumbers = true, bool mustContainLetters = true, int minChars = 8, int maxChars = 16)
        {
            return value.CheckAllCasesIsNotNull() &&
                   ((mustHaveSpecialChars) ? value.ContainsChars(false, "*", "@", "-", "_") : true) &&
                   ((mustContainNumbers) ? value.Any(d => Char.IsDigit(d)) : true) &&
                   ((mustContainLetters) ? value.Any(d => Char.IsLetter(d)) : true) &&
                   ((minChars > 0) ? value.Count() >= minChars : true) &&
                   ((maxChars > 0) ? value.Count() <= maxChars : true);
        }
    }
}
