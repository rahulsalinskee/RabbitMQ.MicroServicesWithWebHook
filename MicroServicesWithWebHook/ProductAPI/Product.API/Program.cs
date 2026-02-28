using Product.API.AttackPrevention.RateLimitForDDoS;
using Product.API.DataLayer;
using Product.API.Exception;
using Product.API.LogConfiguration;
using Serilog;
using System.Runtime.CompilerServices;

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

    // Add services to the container.
    builder.Services.AddControllers();

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    /* Register Rate Limiting Services */
    builder.Services.AddRateLimitingServicesExtension();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwaggerUI(option =>
        {
            option.SwaggerEndpoint(url: "/openapi/v1.json", name: "Product API V1");
        });
    }

    app.UseMiddleware<GlobalExceptionHandler>();
    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging(); // Logs HTTP requests automatically

    /* --- Enable Rate Limiting Middleware : START --- */
    /* Place this AFTER HttpsRedirection but BEFORE Authorization/Controllers */
    app.UseRateLimiter();
    /* --- Enable Rate Limiting Middleware : END --- */


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