using MyAuth.Utils.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyAuth.Utils
{
    public class RequestValidatorPartsHelper
    {
        public string Token { get; set; }
        private MemoryCacheCustomKeysHandler _cache;

        public RequestValidatorPartsHelper(MemoryCacheCustomKeysHandler cache)
        {
            _cache = cache;
        }

        public string CombineAndSaveHash(string value, Guid? guid = null)
        {
            Guid TheGuid;
            if (!guid.HasValue || (guid.HasValue && guid.Value == Guid.Empty))
            {
                TheGuid = Guid.NewGuid();
            }
            else
            {
                TheGuid = guid.Value;
            }
            var finalStrHash = $"{TheGuid.ToString()}::{value}::{DateTime.Now}";

            var valRes = ToBase64Str(finalStrHash);

            return _cache.StoreInMemoryAbsoluteCustomKey<string>(TheGuid.ToString(), valRes, 60 * 60);
        }

        public string CombineAndSaveHashListValues(List<string> values, Guid? guid = null)
        {
            Guid TheGuid;
            if (!guid.HasValue || (guid.HasValue && guid.Value == Guid.Empty))
            {
                TheGuid = Guid.NewGuid();
            }
            else
            {
                TheGuid = guid.Value;
            }
            var firstStrHash = $"{TheGuid.ToString()}";
            foreach (var item in values)
            {
                firstStrHash = $"{firstStrHash}::{item}";
            }
            var finalStrHash = $"{firstStrHash}::{DateTime.Now}";

            var valRes = ToBase64Str(finalStrHash);

            return _cache.StoreInMemoryAbsoluteCustomKey<string>(TheGuid.ToString(), valRes, 60 * 60);
        }

        public bool RetrieveValidateDiscardHash(string hashStr, bool discard = false)
        {
            if (hashStr == null)
            {
                goto FailureCase;
            }
            try
            {
                var main = Base64StringDecode(hashStr)?.Split("::").ToList() ?? null;

                var res = _cache.GetFromMemoryCustomKey<string>(main.First());

                if (res == null)
                {
                    return false;
                }

                if (res != null && discard)
                {
                    _cache.RemoveFromMemoryCustomKey(main.First());
                }
                return res == hashStr;
            }
            catch (Exception)
            {
                goto FailureCase;
            }


        FailureCase: return false;
        }

        public List<string> RetrieveHashValues(string hashStr) => Base64StringDecode(hashStr)?.Split("::").ToList() ?? null;

        public RequestHashParts RetrieveHashValuesObject(string hashStr)
        {
            return null;
        }

        private string ToBase64Str(string str) => System.Convert.ToBase64String(Encoding.UTF8.GetBytes(str));

        private string Base64StringDecode(string encodedString)
        {
            try
            {
                var bytes = Convert.FromBase64String(encodedString);

                var decodedString = Encoding.UTF8.GetString(bytes);

                return decodedString;
            }
            catch (Exception)
            {
                return null;
            }

        }

    }

    public class HMACSHA1SignData
    {
        public string SignData(string message, string secret)
        {
            var encoding = new System.Text.UTF8Encoding();
            var keyBytes = encoding.GetBytes(secret);
            var messageBytes = encoding.GetBytes(message);
            using (var hmacsha1 = new HMACSHA1(keyBytes))
            {
                var hashMessage = hmacsha1.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashMessage);
            }
        }
    }


    public class RequestHashParts
    {
        public string Hash { get; set; }
        public string GuidString { get; set; }
        public string Value { get; set; }
        public DateTime DateTimeCreated { get; set; }
    }
}
