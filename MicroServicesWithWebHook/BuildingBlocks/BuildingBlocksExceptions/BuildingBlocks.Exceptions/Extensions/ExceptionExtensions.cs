using BuildingBlocks.Exceptions.ExceptionHandler;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Exceptions.Extensions
{
    public static class ExceptionExtensions
    {

        public static IServiceCollection AddGlobalExceptionHandlerExtension(this IServiceCollection services)
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
            return services;
        }

        public static IApplicationBuilder UseGlobalExceptionHandlerExtension(this IApplicationBuilder application)
        {
            application.UseExceptionHandler();
            return application;
        }
    }
}
