using Authentication.API.DataContext;
using Authentication.API.Register;
using Authentication.API.Repositories.Implementations;
using Authentication.API.Repositories.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterAuthenticationServiceExtension(configuration: builder.Configuration);

builder.Services.RegisterAuthenticationDbContextExtension(configuration: builder.Configuration);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IJwtService, JwtImplementation>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
