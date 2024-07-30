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

            // Here you would typically generate a JWT token
            // For now, we'll just return the user
            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserInput userInput)
        {
            var result = await authService.RegisterAsync(new AdminDto
            {
                Email = userInput.Email,
                HashedPassword = userInput.HashedPassword,
                LastName = userInput.LastName,
                FirstName = userInput.FirstName,
                DateOfBirth = userInput.DateOfBirth

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