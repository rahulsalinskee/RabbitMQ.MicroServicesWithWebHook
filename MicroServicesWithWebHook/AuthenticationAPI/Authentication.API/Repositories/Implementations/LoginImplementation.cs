using Authentication.API.Repositories.Services;
using Microsoft.AspNetCore.Identity;
using Shared.Data.DTOs.AuthenticationDTOs;
using Shared.Data.DTOs.ResponseDTOs;
using Shared.Data.Models.AuthenticationModel;
using Shared.Data.Models.ErrorModel;

namespace Authentication.API.Repositories.Implementations
{
    public class LoginImplementation : ILoginService
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;

        public LoginImplementation(UserManager<User> userManager, IJwtService jwtService)
        {
            this._userManager = userManager;
            this._jwtService = jwtService;
        }

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

            var userDetail = await this._userManager.FindByNameAsync(loginDto.UserName);

            if (userDetail is not null)
            {
                var isPasswordMatched = await this._userManager.CheckPasswordAsync(user: userDetail, password: loginDto.Password);

                if (isPasswordMatched)
                {
                    string jwtToken = this._jwtService.GenerateToken(userDetail);

                    return new ResponseDto()
                    {
                        Result = jwtToken,
                        IsSuccess = true,
                        When = DateTime.UtcNow,
                        Message = "JWT Token Created Successfully",
                    };
                }

                ApplicationError passwordMismatch = new()
                {
                    Message = "Password Is Mismatched!",
                    When = DateTime.UtcNow,
                };

                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    When = passwordMismatch.When,
                    Message = passwordMismatch.Message,
                };
            }

            ApplicationError loginError = new()
            {
                Message = "User not found",
                When = DateTime.UtcNow,
            };

            return new ResponseDto()
            {
                Result = null,
                IsSuccess = false,
                When = loginError.When,
                Message = loginError.Message,
            };
        }
    }
}
