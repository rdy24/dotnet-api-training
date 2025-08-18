using Microsoft.AspNetCore.Mvc;
using CinemaApi.DTOs.Auth;
using CinemaApi.Interfaces;
using CinemaApi.Responses;
using CinemaApi.Filters;

namespace CinemaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(LoginDto loginDto)
        {
            _logger.LogInformation("API: Login request for username: {Username}", loginDto.Username);

            var authResponse = await _authService.LoginAsync(loginDto);
            if (authResponse == null)
            {
                throw new UnauthorizedAccessException("Login failed");
            }

            var response = ApiResponse<AuthResponse>.SuccessResult(authResponse, "Login successful");
            _logger.LogInformation("API: Login successful for user: {Username}", loginDto.Username);
            
            return Ok(response);
        }
    }
}