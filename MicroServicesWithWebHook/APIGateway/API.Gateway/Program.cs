var builder = WebApplication.CreateBuilder(args);

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

/* Map YARP Middleware */
app.MapReverseProxy();

app.UseAuthorization();

app.MapControllers();

app.Run();
