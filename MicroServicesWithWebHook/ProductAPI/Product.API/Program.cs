using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Product.API.AttackPrevention.CORS;
using Product.API.AttackPrevention.CSRF;
using Product.API.AttackPrevention.RateLimitForDDoS;
using Product.API.Authentication;
using Product.API.Authorization;
using Product.API.Cache.ConfigurationCache;
using Product.API.DataLayer;
using Product.API.Exception;
using Product.API.LogConfiguration;
using Product.API.RabbitMqPublisher;
using Product.API.Repository.CacheServices.Implementations;
using Product.API.Repository.CacheServices.Services;
using Product.API.Repository.FilterServices.Implementations;
using Product.API.Repository.FilterServices.Services;
using Product.API.Version;
using Serilog;
using ProductServiceVersion1 = Product.API.Repository.ProductServices.Version1;
using ProductServiceVersion2 = Product.API.Repository.ProductServices.Version2;

// Initialize logger first to capture startup errors
Log.Logger = ProductLog.GenerateProductLog();

try
{
    Log.Information("Starting Product.API...");

    var builder = WebApplication.CreateBuilder(args);

    /* Integrate Serilog with the ASP.NET Core host */
    builder.Host.UseSerilog();

    /* Register ProductDbContext */
    builder.Services.RegisterProductDbContextExtension(configuration: builder.Configuration);

    /* Register Cache */
    builder.Services.ConfigureCacheExtension(configuration: builder.Configuration);

    // Add services to the container.
    builder.Services.AddControllers();

    /* Register Version Services FIRST so OpenAPI can discover the endpoints */
    builder.Services.RegisterApiVersionExtension();

    builder.Services.AddSwaggerGenAuthorizationExtension();

    /* Register Rate Limiting Services */
    builder.Services.AddRateLimitingServicesExtension();

    /* --- AntiForgery START: Configure Services For CSRF Attack Prevention --- */
    builder.Services.AddAppAntiForgeryExtension();

    /* -- CORS START: Service Registration */
    builder.Services.AddCorsServicesExtension(builder.Configuration);
    /* -- CORS STOP: Service Registration */

    /* --- Register Product Services & Implementations --- */
    builder.Services.AddScoped<ProductServiceVersion1.Services.IProductService, ProductServiceVersion1.Implementations.ProductServiceImplementation>();
    builder.Services.AddScoped<ProductServiceVersion2.Services.IProductService, ProductServiceVersion2.Implementations.ProductServiceImplementation>();

    /* --- Register Cache Services & Implementations --- */
    builder.Services.AddScoped<ICacheService, CacheServiceImplementation>();

    /* --- Register Filter Services & Implementations --- */
    builder.Services.AddScoped<IFilterService<Shared.Data.Models.ProductModel.Product>, FilterImplementation<Shared.Data.Models.ProductModel.Product>>();

    /* Register Product MassTransit as Publisher */
    builder.Services.RegisterProductMassTransitExtension();

    /* Register JWT Authentication */
    builder.Services.AddJwtAuthenticationExtension(configuration: builder.Configuration);

    var app = builder.Build();

    var versionDescriptionProviders = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(option =>
        {
            /* Iterate over the API versions to generate the correct endpoint URLs */
            foreach (var description in versionDescriptionProviders.ApiVersionDescriptions)
            {
                option.SwaggerEndpoint
                (
                    url: $"/swagger/{description.GroupName}/swagger.json", name: $"Product API {description.GroupName.ToUpperInvariant()}"
                );
            }
        });
    }

    app.UseMiddleware<GlobalExceptionHandler>();
    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging(); /* Logs HTTP requests automatically */

    /* --- CORS Middleware START --- */
    /* Must be between UseRouting and UseRateLimiter/UseAuthorization */
    app.UseCorsMiddlewareExtension();
    /* --- CORS Middleware END --- */

    /* --- AntiForgery MiddleWare START --- */
    /* This MiddleWare provides the token to the client via a cookie */
    app.UseAntiForgeryTokenMiddlewareExtension();

    /* --- Enable Rate Limiting MiddleWare : START --- */
    /* Place this AFTER HttpsRedirection but BEFORE Authorization/Controllers */
    app.UseRateLimiter();
    /* --- Enable Rate Limiting MiddleWare : END --- */

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Product.API failed to start.");
}
finally
{
    Log.CloseAndFlush(); // Ensures all logs are written before app exits
}