using BuildingBlocks.Exceptions.Extensions;
using EmailNotification.WebHook.Authentication;
using EmailNotification.WebHook.RabbitMqConsumer;
using EmailNotification.WebHook.Repositories.Implementations;
using EmailNotification.WebHook.Repositories.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Shared.Data.DTOs.EmailDTOs;

/* 1. Initialize a basic bootstrap logger to catch startup errors */
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    Log.Information("Starting EmailNotification.WebHook...");

    var builder = WebApplication.CreateBuilder(args);

    /* 2. Tell ASP.NET Core to use the global Serilog instance */
    builder.Host.UseSerilog();

    /* 3. Centralized Exception Handling */
    builder.Services.AddGlobalExceptionHandlerExtension();

    builder.Services.AddHttpClient();
    builder.Services.AddScoped<IEmailService, EmailServiceImplementation>();

    /* Register Web Hook Consumer */
    builder.Services.RegisterWebHookConsumerMassTransitExtension();

    /* Register Authentication */
    builder.Services.AddJwtAuthenticationExtension(builder.Configuration);

    var app = builder.Build();

    /* 4. Use Centralized Exceptions in the pipeline */
    app.UseGlobalExceptionHandlerExtension();

    app.UseHttpsRedirection();

    /* Logs incoming HTTP webhook requests */
    app.UseSerilogRequestLogging();

    /* Enable Authentication & Authorization */
    app.UseAuthentication();
    app.UseAuthorization();

    /* Minimal API to send email */
    app.MapPost(pattern: "/email-webhook", handler: ([FromBody] EmailDto emailDto, IEmailService emailService) =>
    {
        string result = emailService.SendEmail(emailDto: emailDto);
        return Task.FromResult(result);
    }).RequireAuthorization();

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
