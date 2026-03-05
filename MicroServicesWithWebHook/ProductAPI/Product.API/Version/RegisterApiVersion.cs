using Microsoft.AspNetCore.Mvc;

namespace Product.API.Version
{
    public static class RegisterApiVersion
    {
        public static void  RegisterApiVersionExtension(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(majorVersion: 1, minorVersion: 0);
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.ConfigureOptions<VersionConfiguration>();
        }
    }
}
