using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wu17Picks.Infrastructure.Extensions
{
    public static class CacheHelper
    {
        public static bool SetValue<T>(this IDistributedCache cache, string key, T value)
        {
            cache.SetString(key, JsonConvert.SerializeObject(value));
            return true;
        }

        public static T GetValue<T>(this IDistributedCache cache, string key)
        {
            var value = cache.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
