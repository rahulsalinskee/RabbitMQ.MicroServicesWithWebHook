using Authentication.API.DataContext;
using Authentication.API.Register;
using Authentication.API.Repositories.Implementations;
using Authentication.API.Repositories.Services;
using Microsoft.AspNetCore.Identity;
using Shared.Data.Models.AuthenticationModel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterAuthenticationServiceExtension(configuration: builder.Configuration);

builder.Services.RegisterAuthenticationDbContextExtension(configuration: builder.Configuration);

/* ADD Add Identity */
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<AuthenticationDbContext>().AddDefaultTokenProviders();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IJwtService, JwtImplementation>();
builder.Services.AddScoped<ILoginService, LoginImplementation>();
builder.Services.AddScoped<IRegisterService, RegisterImplementation>();

var app = builder.Build();

/* --- ADDED THIS BLOCK TO SEED ROLES --- */
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Execute the seeder asynchronously 
        await RoleSeeder.SeedRoleAsyncExtension(services);
    }
    catch (Exception exception)
    {
        // Log any errors that occur during the database seeding process
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, "An error occurred while seeding roles to the database.");
    }
}
/* ------------------------------------ */

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(option =>
    {
        option.SwaggerEndpoint(url: "/openapi/v1.json", name: "Authentication API");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
