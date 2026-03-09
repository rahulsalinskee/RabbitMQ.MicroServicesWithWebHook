namespace Product.API.Cache.ConfigurationCache
{
    public static class RegisterCache
    {
        public static IServiceCollection ConfigureCacheExtension(this IServiceCollection services, IConfiguration configuration)
        {
            /* --- Configure Redis Cache --- */
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString(name: "RedisCacheConnectionString");

                /* Prefix for keys to avoid collision if other apps use the same Redis instance */
                options.InstanceName = "ProductAPI_";
            });

            return services;
        }
    }
}
