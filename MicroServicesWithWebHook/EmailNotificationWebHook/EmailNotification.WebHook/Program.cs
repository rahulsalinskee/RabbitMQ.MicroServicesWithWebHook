using Microsoft.AspNetCore.Mvc;
using Shared.Data.DTOs.EmailDTOs;
using EmailNotification.WebHook.Repositories.Services;
using EmailNotification.WebHook.Repositories.Implementations;
using MassTransit;
using EmailNotification.WebHook.RabbitMqConsumer;
using EmailNotification.WebHook.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddScoped<IEmailService, EmailServiceImplementation>();

/* Register Web Hook Consumer */
builder.Services.RegisterWebHookConsumerMassTransitExtension();

/* Register Authentication */
builder.Services.AddJwtAuthenticationExtension(builder.Configuration);

var app = builder.Build();

/* Enable Authentication */
app.UseAuthentication();

/* Enable Authorization */
app.UseAuthorization();

/* Minimal API to send email */
app.MapPost(pattern: "/email-webhook", handler: ([FromBody] EmailDto emailDto, IEmailService emailService) =>
{
    string result = emailService.SendEmail(emailDto: emailDto);

    return Task.FromResult(result);
}).RequireAuthorization();

app.Run();
