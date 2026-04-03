using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Product.API.Version
{
    public class VersionConfiguration : IConfigureNamedOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;

        public VersionConfiguration(IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            this._apiVersionDescriptionProvider = apiVersionDescriptionProvider;
        }

        public void Configure(string? name, SwaggerGenOptions options)
        {
            Configure(options: options);
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in this._apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(name: description.GroupName, info: CreateVersionInformation(description: description));
            }
        }

        private static OpenApiInfo CreateVersionInformation(ApiVersionDescription description)
        {
            return new OpenApiInfo()
            {
                Title = $"Product API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "API for managing products with versioning support.",
            };
        }
    }
}
