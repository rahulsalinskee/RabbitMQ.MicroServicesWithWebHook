using Authentication.API.Repositories.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Data.DTOs.AuthenticationDTOs;

namespace Authentication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IRegisterService _registerService;
        private readonly ILoginService _loginService;

        public AuthenticationController(IRegisterService registerService, ILoginService loginService)
        {
            this._registerService = registerService;
            this._loginService = loginService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterNewUser([FromBody] RegisterDto registerDto)
        {
            var registerResponse = await this._registerService.RegisterNewUserAsync(registerDto);

            if (registerResponse.IsSuccess)
            {
                return Ok(registerResponse);

            }
            return BadRequest(registerResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginExistingUser([FromBody] LoginDto loginDto)
        {
            var loginResponse = await this._loginService.LoginAsync(loginDto);
            if (loginResponse.IsSuccess)
            {
                return Ok(loginResponse);
            }
            return BadRequest(loginResponse);
        }
    }
}
