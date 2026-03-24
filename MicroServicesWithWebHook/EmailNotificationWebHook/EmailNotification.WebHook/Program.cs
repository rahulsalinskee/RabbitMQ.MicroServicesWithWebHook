using Microsoft.AspNetCore.Mvc;
using Shared.Data.DTOs.EmailDTOs;
using EmailNotification.WebHook.Repositories.Services;
using EmailNotification.WebHook.Repositories.Implementations;
using MassTransit;
using EmailNotification.WebHook.RabbitMqConsumer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IEmailService, EmailServiceImplementation>();

/* Register Web Hook Consumer */
builder.Services.RegisterWebHookConsumerMassTransitExtension();

var app = builder.Build();

/* Minimal API to send email */
app.MapPost(pattern: "/email-webhook", handler: ([FromBody] EmailDto emailDto, IEmailService emailService) =>
{
    string result = emailService.SendEmail(emailDto: emailDto);

    return Task.FromResult(result);
});

app.Run();
