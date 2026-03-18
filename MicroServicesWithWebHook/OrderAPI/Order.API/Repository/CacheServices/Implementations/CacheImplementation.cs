using Microsoft.Extensions.Caching.Distributed;
using Order.API.Repository.CacheServices.Services;
using System.Text.Json;

namespace Order.API.Repository.CacheServices.Implementations
{
    public class CacheImplementation : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        public CacheImplementation(IDistributedCache distributedCache)
        {
            this._distributedCache = distributedCache;
        }

        public async Task<T?> GetDataAsync<T>(string key)
        {
            var cacheData = await this._distributedCache.GetStringAsync(key);

            if (string.IsNullOrEmpty(cacheData))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(cacheData);
        }

        public async Task RemoveDataAsync(string key)
        {
            await this._distributedCache.RemoveAsync(key);
        }

        public Task SetDataAsync<T>(string key, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions();

            if (absoluteExpireTime.HasValue)
            {
                /* Absolute Expiration: Cache expires strictly after this time */
                options.AbsoluteExpirationRelativeToNow = absoluteExpireTime;
            }
            else if (slidingExpireTime.HasValue)
            {
                /* Sliding Expiration: Cache expiration resets if accessed within this time window */
                options.SlidingExpiration = slidingExpireTime;
            }
            else
            {
                /* Default to 5 minutes */
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            }

            var jsonSerializedData = JsonSerializer.Serialize(data);
            return this._distributedCache.SetStringAsync(key: key, value: jsonSerializedData, options: options);
        }
    }
}
