using Business.Abstract;
using Business.Constants;
using Entities.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]s")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register(UserForRegister userForRegister)
        {
            var userExist = _authService.UserExist(userForRegister.Email);
            if (!userExist.Success)
            {
                return BadRequest(userExist.Message);
            }

            var registerResult = _authService.Register(userForRegister, userForRegister.Password);
            var result = _authService.CreateAccessToken(registerResult.Data, 0); 
            if (result.Success)
            {
                return Ok(result.Data);
            }

            // if (registerResult.Success)
            // {
            //     return Ok(registerResult);
            // }

            return BadRequest(registerResult.Message);
        }

        [HttpPost("login")]
        public IActionResult Login(UserForLogin userForLogin)
        {
            var userToLogin = _authService.Login(userForLogin);
            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin);
            }

            var result = _authService.CreateAccessToken(userToLogin.Data,1);
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }
    }
}