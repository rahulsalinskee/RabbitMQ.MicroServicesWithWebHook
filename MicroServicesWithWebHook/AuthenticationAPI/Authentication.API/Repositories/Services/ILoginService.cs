using Shared.Data.DTOs.AuthenticationDTOs;
using Shared.Data.DTOs.ResponseDTOs;

namespace Authentication.API.Repositories.Services
{
    public interface ILoginService
    {
        public Task<ResponseDto> LoginAsync(LoginDto loginDto);
    }
}
