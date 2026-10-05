using System.Security.Claims;
using GSEvent.Common;
using GSEvent.Common.Pagination;
using GSEvent.DTOs.Auth;
using GSEvent.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [HttpGet("get-roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _authService.GetAllRolesAsync();
            if (roles == null || roles.Count == 0)
            {
                return NotFound(
                    ApiResponse<object>.ErrorResponse(
                        "No roles found",
                        StatusCodes.Status404NotFound,
                        new[] { "No roles found" }
                    )
                );
            }
            return Ok(
                ApiResponse<List<ReadRoleDto>>.SuccessResponse(
                    roles,
                    "Roles retrieved successfully",
                    StatusCodes.Status200OK
                )
            );
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = await _authService.RegisterAsync(dto);
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
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string username, [FromQuery] string token)
        {
            var dto = new EmailVerificationDto
            {
                Username = username,
                Token = token
            };
            var result = await _authService.VerifyEmailAsync(dto);
            if (!result)
            {
                return BadRequest(
                    ApiResponse<object>.ErrorResponse(
                        "Email verification failed",
                        StatusCodes.Status400BadRequest,
                        new[] { "Invalid or expired verification token." }
                    )
                );
            }

            return Ok(
                ApiResponse<object>.SuccessResponse(
                    null,
                    "Email verified successfully."
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
        [Authorize]
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

        [Authorize]
        [HttpGet("get-users")]
        public async Task<IActionResult> GetUsers([FromQuery] PaginationRequest request)
        {
            var users = await _authService.GetAllUsersAsync(request);
            return Ok(
                ApiResponse<PaginationResponse<UserReadDto>>.SuccessResponse(
                    users,
                    "Users retrieved successfully",
                    StatusCodes.Status200OK
                )
            );
        }

    }
}
