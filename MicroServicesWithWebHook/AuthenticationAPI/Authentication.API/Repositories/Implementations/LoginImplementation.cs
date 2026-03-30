using Authentication.API.Repositories.Services;
using Shared.Data.DTOs.AuthenticationDTOs;
using Shared.Data.DTOs.ResponseDTOs;
using Shared.Data.Models.ErrorModel;

namespace Authentication.API.Repositories.Implementations
{
    public class LoginImplementation : ILoginService
    {
        public async Task<ResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (loginDto is null)
            {
                ApplicationError applicationError = new()
                {
                    Message = "Login detail is null",
                    When = DateTime.UtcNow,
                };

                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    When = applicationError.When,
                    Message = applicationError.Message,
                };
            }

            /* Fake User Demonstration */
            if (loginDto.UserName == "admin" && loginDto.Password == "Password")
            {

            }

            return new ResponseDto()
            {
                Result = null,
                IsSuccess = false,
                When = DateTime.UtcNow,
                Message = "User not found",
            };
        }
    }
}
