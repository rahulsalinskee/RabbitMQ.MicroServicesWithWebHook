using API.Gateway.Authentication;
using API.Gateway.Authorization;
using API.Gateway.Fallback;
using BuildingBlocks.Exceptions.Extensions;
using BuildingBlocks.Logging.LogFactory;
using Serilog;

/* 1. Initialize a basic bootstrap logger to catch startup errors */
Log.Logger = SharedLogFactory.GenerateSharedLogger(applicationName: "Gateway");

try
{
    Log.Information("Starting Gateway.API...");

    var builder = WebApplication.CreateBuilder(args);

    /* 2. Tell ASP.NET Core to use the global Serilog instance we just configured
    *  Centralized Logging (Replaces the old ProductLog and local UseSerilog) 
    */
    builder.Host.UseSerilog();

    /* 3. Centralized Exception Handling - Service Registration */
    builder.Services.AddGlobalExceptionHandlerExtension();

    /* Register Authentication */
    builder.Services.RegisterAuthenticationExtension(builder.Configuration);

    /* Register Authorization */
    builder.Services.RegisterAuthorizationExtensions();

    /* Register YARP with Resilience */
    builder.Services.RegisterYarpWithResilienceExtension(configuration: builder.Configuration);

    /* Add YARP Reverse Proxy Services */
    //builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    // Add services to the container.
    builder.Services.AddControllers();

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    /* 4. Use Centralized Exception */
    app.UseGlobalExceptionHandlerExtension();

    /* Use Gateway Fallback Middleware */
    app.UseMiddleware<GatewayFallbackMiddleware>();

    app.UseHttpsRedirection();

    /* MUST BE IN THIS ORDER */
    app.UseAuthentication();
    app.UseAuthorization();

    /* Map YARP Middleware */
    app.MapReverseProxy();

    app.MapControllers();

    app.Run();
}
catch (Exception exception) when (exception is not HostAbortedException)
{
    Log.Fatal(exception, "Gateway.API failed to start.");
}
finally
{
    /* Ensures all logs are written before app exits */
    Log.CloseAndFlush();
}