using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Product.API.Authentication
{
    public static class RegisterAuthentication
    {
        /// <summary>
        /// Add JWT Authentication Validation
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddJwtAuthenticationExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["JWT:Key"]!)),
                    RoleClaimType = "Role"
                };
            });
            services.AddAuthorization();
        }
    }
}
