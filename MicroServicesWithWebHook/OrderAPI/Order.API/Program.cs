using Order.API.DataLayer;
using Order.API.GlobalException;
using Order.API.LogConfiguration;
using Serilog;

// Initialize logger first to capture startup errors
Log.Logger = OrderLog.GenerateOrderLog();

try
{
    Log.Information("Starting Order.API...");

    var builder = WebApplication.CreateBuilder(args);

    /* Integrate Serilog with the ASP.NET Core host */
    builder.Host.UseSerilog();

    /* Register OrderDbContext */
    builder.Services.RegisterOrderDbContextExtension(configuration: builder.Configuration);

    // Add services to the container.
    builder.Services.AddControllers();

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwaggerUI(option =>
        {
            option.SwaggerEndpoint(url: "/openapi/v1.json", name: "Order API V1");
        });
    }

    app.UseMiddleware<GlobalExceptionHandler>();
    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging(); // Logs HTTP requests automatically
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