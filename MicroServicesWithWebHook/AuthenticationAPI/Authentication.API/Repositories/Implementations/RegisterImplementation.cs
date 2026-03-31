using Authentication.API.Repositories.Services;
using Microsoft.AspNetCore.Identity;
using Shared.Data.DTOs.AuthenticationDTOs;
using Shared.Data.DTOs.ResponseDTOs;
using Shared.Data.Models.ErrorModel;

namespace Authentication.API.Repositories.Implementations
{
    public class RegisterImplementation : IRegisterService
    {
        private readonly UserManager<IdentityUser> _identityUserManager;

        public RegisterImplementation(UserManager<IdentityUser> identityUserManager)
        {
            this._identityUserManager = identityUserManager;
        }

        public async Task<ResponseDto> RegisterNewUserAsync(RegisterDto registerDto)
        {
            if (registerDto is null)
            {
                ApplicationError registerDtoError = new()
                {
                    Message = "Registration data is blank!",
                    When = DateTime.UtcNow
                };

                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    When = registerDtoError.When,
                    Message = registerDtoError.Message
                };
            }

            IdentityUser identityUser = new()
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName
            };

            var addNewUserResponse = await this._identityUserManager.CreateAsync(user: identityUser, password: registerDto.Password);

            if (addNewUserResponse.Succeeded)
            {
                if (registerDto.Role is not null)
                {
                    var identityUserResult = await this._identityUserManager.AddToRoleAsync(user: identityUser, role: registerDto.Role);

                    if (identityUserResult.Succeeded)
                    {
                        return new ResponseDto()
                        {
                            Result = null,
                            IsSuccess = true,
                            When = DateTime.UtcNow,
                            Message = "User registered successfully!"
                        };
                    }
                    ApplicationError userRegisteredSuccessfully = new()
                    {
                        Message = "User registered successfully!",
                        When = DateTime.UtcNow
                    };

                    return new ResponseDto()
                    {
                        Result = null,
                        IsSuccess = false,
                        When = userRegisteredSuccessfully.When,
                        Message = userRegisteredSuccessfully.Message
                    };
                }
                ApplicationError error = new()
                {
                    Message = "User registered successfully!",
                    When = DateTime.UtcNow
                };

                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    When = error.When,
                    Message = error.Message
                };
            }
            ApplicationError addNewUserError = new()
            {
                Message = "Failed to register user!",
                When = DateTime.UtcNow
            };
            return new ResponseDto()
            {
                Result = null,
                IsSuccess = false,
                When = addNewUserError.When,
                Message = addNewUserError.Message
            };
        }
    }
}