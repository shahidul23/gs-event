using GSEvent.Common;
using GSEvent.DTOs.Auth;
using GSEvent.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GSEvent.Controllers.Auth
{
    [Route("api")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly IPasswordService _passwordService;
        public PasswordController(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> PasswordChange(
            [FromBody] ChangePasswordDto dto)
        {
            var change = await _passwordService.ChangePasswordAsync(dto);

            if (change == null)
            {
                return BadRequest(
                    ApiResponse<object>.ErrorResponse(
                        "Password Change Failed",
                        StatusCodes.Status400BadRequest,
                        new[] { "Something went wrong." }
                    )
                );
            }

            return Ok(
                ApiResponse<object>.SuccessResponse(
                    null,
                    "Password changed successfully.",
                    StatusCodes.Status200OK
                )
            );
        }

        
    }
}
