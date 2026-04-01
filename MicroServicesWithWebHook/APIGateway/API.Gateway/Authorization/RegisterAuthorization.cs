namespace API.Gateway.Authorization
{
    public static class RegisterAuthorization
    {
        public static void RegisterAuthorizationExtensions(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireClaim("Role", "Admin").RequireAuthenticatedUser());
                options.AddPolicy("User", policy => policy.RequireClaim("Role", "User"));
            });
        }
    }
}
