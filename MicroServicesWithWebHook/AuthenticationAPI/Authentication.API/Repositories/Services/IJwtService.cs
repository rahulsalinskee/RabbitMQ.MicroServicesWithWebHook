using Shared.Data.Models.AuthenticationModel;

namespace Authentication.API.Repositories.Services
{
    public interface IJwtService
    {
        public string GenerateToken(User user);
    }
}
