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

    /* 2. Tell ASP.NET Core to use the global Serilog instance we just configured
    *  Centralized Logging (Replaces the old ProductLog and local UseSerilog) 
    */
    builder.Host.UseSerilog();

    /* 3. Centralized Exception Handling - Service Registration */
    builder.Services.AddGlobalExceptionHandlerExtension();

    /* Register OrderDbContext */
    builder.Services.RegisterOrderDbContextExtension(configuration: builder.Configuration);

    /* Register Health Checks */
    builder.Services.AddHealthChecks();

    /* Register Cache */
    builder.Services.ConfigureCacheExtension(configuration: builder.Configuration);

    // Add services to the container.
    builder.Services.AddControllers();

    /* Registers the OpenAPI document generator */
    builder.Services.AddSwaggerGen();

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
            option.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "Order API V1");
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

    /* 5. Centralized Health Checks - Pipeline Middleware  */
    app.MapHealthChecks("/health");

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