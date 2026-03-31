using Shared.Data.DTOs.AuthenticationDTOs;
using Shared.Data.DTOs.ResponseDTOs;

namespace Authentication.API.Repositories.Services
{
    public interface IRegisterService
    {
        public Task<ResponseDto> RegisterNewUserAsync(RegisterDto registerDto);
    }
}
