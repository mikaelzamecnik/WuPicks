using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wu17Picks.Infrastructure.Helpers
{
    public static class CacheHelper
    {
        public static void SetValue<T>(this IDistributedCache cache, string key, T value)
        {
            cache.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetValue<T>(this IDistributedCache cache, string key)
        {
            var value = cache.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
