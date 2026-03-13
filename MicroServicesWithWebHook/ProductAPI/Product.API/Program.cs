using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Product.API.AttackPrevention.CORS;
using Product.API.AttackPrevention.CSRF;
using Product.API.AttackPrevention.RateLimitForDDoS;
using Product.API.Cache.ConfigurationCache;
using Product.API.DataLayer;
using Product.API.Exception;
using Product.API.LogConfiguration;
using Product.API.Repository.CacheServices.Implementations;
using Product.API.Repository.CacheServices.Services;
using Product.API.Repository.FilterServices.Implementations;
using Product.API.Repository.FilterServices.Services;
using Product.API.Version;
using Serilog;
using Shared.Data.Models.ProductModel;

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

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    // Use LOWERCASE to exactly match the GroupNameFormat "'v'VVV" (v1, v2)
    builder.Services.AddOpenApi("v1");
    builder.Services.AddOpenApi("v2");

    /* Register Rate Limiting Services */
    builder.Services.AddRateLimitingServicesExtension();

    /* --- AntiForgery START: Configure Services For CSRF Attack Prevention --- */
    builder.Services.AddAppAntiForgeryExtension();

    /* -- CORS START: Service Registration */
    builder.Services.AddCorsServicesExtension(builder.Configuration);
    /* -- CORS STOP: Service Registration */

    /* --- Register Product Services & Implementations --- */
    builder.Services.AddScoped<Product.API.Repository.ProductServices.Version1.Services.IProductService, Product.API.Repository.ProductServices.Version1.Implementations.ProductServiceImplementation>();
    builder.Services.AddScoped<Product.API.Repository.ProductServices.Version2.Services.IProductService, Product.API.Repository.ProductServices.Version2.Implementations.ProductServiceImplementation>();

    /* --- Register Cache Services & Implementations --- */
    builder.Services.AddScoped<ICacheService, CacheServiceImplementation>();

    /* --- Register Filter Services & Implementations --- */
    builder.Services.AddScoped<IFilterService<Shared.Data.Models.ProductModel.Product>, FilterImplementation<Shared.Data.Models.ProductModel.Product>>();

    var app = builder.Build();

    var versionDescriptionProviders = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwaggerUI(option =>
        {
            /* Iterate over the API versions to generate the correct endpoint URLs */
            foreach (var description in versionDescriptionProviders.ApiVersionDescriptions)
            {
                /* /openapi/{group}.json */
                option.SwaggerEndpoint
                (
                    url: $"/openapi/{description.GroupName}.json", name: $"Product API {description.GroupName.ToUpperInvariant()}"
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