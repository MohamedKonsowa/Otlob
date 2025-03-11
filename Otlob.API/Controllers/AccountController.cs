using Microsoft.AspNetCore.Identity;
using Otlob.API.Helpers;

namespace Otlob.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(JwtOptions jwtOptions, UserManager<AppUser> userManager) : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);

            if (user is null) return Unauthorized("Invalid email or password");

            var result = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if (result)
            {
                return Ok(new UserDto
                {
                    Email = user.Email ?? string.Empty,
                    DisplayName = user.UserName ?? string.Empty,
                    Token = JwtTokenGenerator.GenerateToken(jwtOptions, user)
                });
            }
            else
            {
                return Unauthorized("Invalid email or password");
            }
        }
    }
}