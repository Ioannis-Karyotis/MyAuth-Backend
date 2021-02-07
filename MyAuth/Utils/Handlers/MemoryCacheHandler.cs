using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Utils.Handlers
{
    public class MemoryCacheHandler<TKeys> where TKeys : Enum
    {
        private readonly ILogger<MemoryCacheHandler<TKeys>> _logger;
        private readonly IMemoryCache _cache;

        public MemoryCacheHandler(IMemoryCache cache, ILogger<MemoryCacheHandler<TKeys>> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public T StoreInMemoryAbsolute<T>(TKeys key, T obj, double secondsToLive = 60) where T : class
        {
            try
            {
                return _cache.Set(key, obj, new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(secondsToLive) });
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message} {e.StackTrace}");
            }
            return null;
        }

        public T GetFromMemory<T>(TKeys key) where T : class
        {
            try
            {
                if (_cache.TryGetValue(key, out T value))
                {
                    return value;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message} {e.StackTrace}");
            }
            return null;
        }

        public void RemoveFromMemoryKey(TKeys key)
        {
            try
            {
                _cache.Remove(key);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message} {e.StackTrace}");
            }
        }
    }

    public class MemoryCacheCustomKeysHandler
    {
        private readonly ILogger<MemoryCacheCustomKeysHandler> _logger;
        private readonly IMemoryCache _cache;

        public MemoryCacheCustomKeysHandler(IMemoryCache cache, ILogger<MemoryCacheCustomKeysHandler> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public T StoreInMemoryAbsoluteCustomKey<T>(string key, T obj, double secondsToLive = 60) where T : class
        {
            try
            {
                return _cache.Set(key, obj, new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(secondsToLive) });
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message} {e.StackTrace}");
            }
            return null;
        }

        public T GetFromMemoryCustomKey<T>(string key) where T : class
        {
            try
            {
                if (_cache.TryGetValue(key, out T value))
                {
                    return value;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message} {e.StackTrace}");
            }
            return null;
        }


        public void RemoveFromMemoryCustomKey(string key)
        {
            try
            {
                _cache.Remove(key);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message} {e.StackTrace}");
            }
        }
    }
}
