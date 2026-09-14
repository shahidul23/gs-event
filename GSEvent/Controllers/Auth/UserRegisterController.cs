using GSEvent.Common;
using GSEvent.DTOs.Auth;
using GSEvent.Services.Interfaces;
using Microsoft.AspNetCore.Http;
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
            if(user == null)
            {
                return BadRequest(
                    ApiResponse<object>.ErrorResponse("Registration failed.", StatusCodes.Status400BadRequest, new[] { "User could not be created." })
                );
            }
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(user, "User Register Successfull", StatusCodes.Status201Created));
        }
    }
}
