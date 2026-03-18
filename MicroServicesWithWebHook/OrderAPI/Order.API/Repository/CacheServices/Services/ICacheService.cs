namespace Order.API.Repository.CacheServices.Services
{
    public interface ICacheService
    {

        public Task<T?> GetDataAsync<T>(string key);

        public Task SetDataAsync<T>(string key, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null);

        public Task RemoveDataAsync(string key);
    }
}