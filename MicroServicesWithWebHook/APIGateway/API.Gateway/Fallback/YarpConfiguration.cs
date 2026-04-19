using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Gateway.Fallback
{
    public static class YarpConfiguration
    {
        public static IServiceCollection RegisterYarpWithResilienceExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddReverseProxy().LoadFromConfig(configuration.GetSection(key: "ReverseProxy"));
            return services;
        }
    }
}
