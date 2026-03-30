using Authentication.API.Repositories.Services;
using Microsoft.IdentityModel.Tokens;
using Shared.Data.Models.AuthenticationModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.API.Repositories.Implementations
{
    public class JwtImplementation : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtImplementation(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            SymmetricSecurityKey symmetricSecurityKey = new(Encoding.UTF8.GetBytes(_configuration["JWT:SecreteKey"]));
            SigningCredentials signingCredentials = new(key: symmetricSecurityKey, algorithm: SecurityAlgorithms.HmacSha256);

            IList<Claim>? claims = new List<Claim>();
            claims.Add(item: new Claim(type: JwtRegisteredClaimNames.Sub, value: user.Id.ToString()));
            claims.Add(item: new Claim(type: ClaimTypes.Name, value: user.UserName));
            claims.Add(item: new Claim(type: ClaimTypes.Role, value: user.Role));
            claims.Add(item: new Claim(type: JwtRegisteredClaimNames.Jti, value: Guid.NewGuid().ToString()));

            JwtSecurityToken jwtSecurityToken = new
            (
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token: jwtSecurityToken);
        }
    }
}
