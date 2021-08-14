using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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

        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
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
