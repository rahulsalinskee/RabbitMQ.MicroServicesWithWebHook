using BuildingBlocks.Exceptions.Extensions;
using BuildingBlocks.Logging.LogFactory;
using Order.API.Authentication;
using Order.API.Authorization;
using Order.API.Cache;
using Order.API.DataLayer;
using Order.API.RabbitMqConsumer.RegisterProductConsumer;
using Order.API.Repository.CacheServices.Implementations;
using Order.API.Repository.CacheServices.Services;
using Order.API.Repository.FilterServices.Implementations;
using Order.API.Repository.FilterServices.Services;
using Order.API.Repository.OrderServices.Implementations;
using Order.API.Repository.OrderServices.Services;
using Serilog;
using OrderModel = Shared.Data.Models.OrderModel.Order;

/* 1. Initialize a basic bootstrap logger to catch startup errors */
Log.Logger = SharedLogFactory.GenerateSharedLogger(applicationName: "Order.API");

try
{
    Log.Information("Starting Order.API...");

    var builder = WebApplication.CreateBuilder(args);

    /* Integrate Serilog with the ASP.NET Core host */
    builder.Host.UseSerilog();

    /* 3. Centralized Exception Handling - Service Registration */
    builder.Services.AddGlobalExceptionHandlerExtension();

    /* Register OrderDbContext */
    builder.Services.RegisterOrderDbContextExtension(configuration: builder.Configuration);

    /* Register Cache */
    builder.Services.ConfigureCacheExtension(configuration: builder.Configuration);

    // Add services to the container.
    builder.Services.AddControllers();

    builder.Services.AddSwaggerGenAuthorizationExtension();

    builder.Services.AddScoped<IOrderService, OrderImplementation>();
    builder.Services.AddScoped<ICacheService, CacheImplementation>();
    builder.Services.AddScoped<IFilterService<OrderModel>, FilterImplementation<OrderModel>>();

    /* --- ADD MASS TRANSIT REGISTRATION HERE --- */
    builder.Services.RegisterProductConsumerMassTransitExtension();

    /* --- ADD JWT AUTHENTICATION REGISTRATION HERE --- */
    builder.Services.AddJwtAuthenticationExtension(configuration: builder.Configuration);

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(option =>
        {
            option.SwaggerEndpoint(url: "/openapi/v1.json", name: "Order API V1");
        });
    }

    /* 4. Centralized Exception Handling - Pipeline Middleware */
    /* Replaces app.UseMiddleware<GlobalExceptionHandler>(); */
    app.UseGlobalExceptionHandlerExtension();

    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging(); // Logs HTTP requests automatically

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception) when (exception is not HostAbortedException)
{
    Log.Fatal(exception, "Order.API failed to start.");
}
finally
{
    Log.CloseAndFlush(); // Ensures all logs are written before app exits
}