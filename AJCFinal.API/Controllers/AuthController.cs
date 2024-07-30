using Microsoft.AspNetCore.Mvc;
using AJCFinal.Business.Abstractions;
using AJCFinal.Business.DataTransfertObjects;
using AJCFinal.API.Models;

namespace AJCFinal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInput loginInput)
        {
            var user = await authService.AuthenticateAsync(loginInput.Email, loginInput.Password);
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }
            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] PersonInput personInput)
        {
            var result = await authService.RegisterAsync(new PersonDto
            {
                Email = personInput.Email,
                HashedPassword = personInput.HashedPassword,
                LastName = personInput.LastName,
                FirstName = personInput.FirstName,
                DateOfBirth = personInput.DateOfBirth,
                Address = personInput.Address,
                Phone = personInput.Phone,
                Interests = personInput.Interests
                
            });

            if (result)
            {
                return Ok("User registered successfully");
            }
            return BadRequest("Registration failed");
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordInput changePasswordInput)
        {
            var result = await authService.ChangePasswordAsync(changePasswordInput.UserId,
                                                               changePasswordInput.OldPassword, changePasswordInput.NewPassword);
            if (result)
            {
                return Ok("Password changed successfully");
            }
            return BadRequest("Failed to change password");
        }

    }
}