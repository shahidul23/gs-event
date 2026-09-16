using GSEvent.Common;
using GSEvent.DTOs.Auth;
using GSEvent.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GSEvent.Controllers.Auth
{
    [Route("api")]
    [ApiController]
    public class UserRegisterController : ControllerBase
    {
        private IAuthService _authService;
        public UserRegisterController(
            IAuthService authService
        )
        {
            _authService = authService; 
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = await _authService.RegisterAsync(dto);
            Console.WriteLine(user);
            if(user == null)
            {
                return BadRequest(
                    ApiResponse<object>.ErrorResponse("Registration failed.", StatusCodes.Status400BadRequest, new[] { "User could not be created." })
                );
            }
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<UserReadDto>.SuccessResponse(
                    user,
                    "User registered successfully.",
                    StatusCodes.Status201Created
                )
            );
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _authService.LoginAsync(dto);
            if (user == null)
            {
                return Unauthorized(
                    ApiResponse<object>.ErrorResponse("Login Failed", StatusCodes.Status401Unauthorized, new [] {"Invalid username/email or password." })
                );
            }
            return Ok(
                ApiResponse<AuthResponseDto>.SuccessResponse(user, "Login Successfull", StatusCodes.Status200OK)
            );
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult>RefreshToken([FromBody] TokenResetDto token)
        {
            var responce = await _authService.VerifyAndGenerateTokenAsync(token);
            if (responce == null)
            {
                return BadRequest(
                    ApiResponse<object>.ErrorResponse(
                        "Token refresh failed.",
                        StatusCodes.Status400BadRequest,
                        new[] 
                        {
                            "Invalid or expired refresh token."
                        }
                    )
                );
            }
            return Ok(
                ApiResponse<AuthResponseDto>.SuccessResponse(
                    responce,
                    "Token refreshed successfully.",
                    StatusCodes.Status200OK
                )
            );
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDto logout)
        {
            var result = await _authService.LogoutAsync(logout);
            if (!result)
            {
                return BadRequest(
                    ApiResponse<object>.ErrorResponse(
                        "Invalid or already revoked refresh token",
                        StatusCodes.Status400BadRequest,
                        new[]
                        {"Logout Failed"}
                    )
                );
            }
            return Ok(
                ApiResponse<object>.SuccessResponse(
                    null,
                    "Logout successful",
                    StatusCodes.Status200OK
                )
            );
        }
    }
}
