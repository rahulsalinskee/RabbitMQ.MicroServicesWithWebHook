using API.Gateway.Authentication;
using API.Gateway.Authorization;

var builder = WebApplication.CreateBuilder(args);

/* Register Authentication */
builder.Services.RegisterAuthenticationExtension(builder.Configuration);

/* Register Authorization */
builder.Services.RegisterAuthorizationExtensions();

/* Add YARP Reverse Proxy Services */
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

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

app.UseHttpsRedirection();

/* MUST BE IN THIS ORDER */
app.UseAuthentication();
app.UseAuthorization();

/* Map YARP Middleware */
app.MapReverseProxy();

app.UseAuthorization();

app.MapControllers();

app.Run();
