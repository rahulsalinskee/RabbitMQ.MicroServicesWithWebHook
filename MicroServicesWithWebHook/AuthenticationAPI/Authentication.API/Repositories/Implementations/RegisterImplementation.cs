using Authentication.API.Repositories.Services;
using Microsoft.AspNetCore.Identity;
using Shared.Data.DTOs.AuthenticationDTOs;
using Shared.Data.DTOs.ResponseDTOs;
using Shared.Data.Models.AuthenticationModel;
using Shared.Data.Models.ErrorModel;

namespace Authentication.API.Repositories.Implementations
{
    public class RegisterImplementation : IRegisterService
    {
        private readonly UserManager<User> _userManager;

        public RegisterImplementation(UserManager<User> userManager)
        {
            this._userManager = userManager;
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

            User identityUser = new()
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                Role = registerDto.Role ?? "User"
            };

            
            var addNewUserResponse = await this._userManager.CreateAsync(user: identityUser, password: registerDto.Password);

            if (addNewUserResponse.Succeeded)
            {
                if (registerDto.Role is not null)
                {
                    var identityUserResult = await this._userManager.AddToRoleAsync(user: identityUser, role: registerDto.Role);

                    if (identityUserResult.Succeeded)
                    {
                        return new ResponseDto()
                        {
                            Result = identityUserResult,
                            IsSuccess = true,
                            When = DateTime.UtcNow,
                            Message = "User registered successfully!"
                        };
                    }

                    /* IF IT FAILS: Extract the actual Identity errors */
                    var identityErrors = string.Join(", ", addNewUserResponse.Errors.Select(error => error.Description));

                    ApplicationError newUserError = new()
                    {
                        // Append the specific Identity errors to your message
                        Message = $"Failed to register user! Reasons: {identityErrors}",
                        When = DateTime.UtcNow
                    };

                    return new ResponseDto()
                    {
                        Result = null,
                        IsSuccess = false,
                        When = newUserError.When,
                        Message = newUserError.Message
                    };
                }

                ApplicationError error = new()
                {
                    Message = "User is not registered!",
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