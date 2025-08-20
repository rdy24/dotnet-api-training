using Microsoft.AspNetCore.Mvc;
using CinemaApi.DTOs.Auth;
using CinemaApi.Interfaces;
using CinemaApi.Responses;
using CinemaApi.Filters;
using Sentry;

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

        [HttpGet("test-sentry")]
        public IActionResult TestSentry()
        {
            _logger.LogInformation("Testing Sentry integration");
            
            // Test different Sentry features
            SentrySdk.CaptureMessage("Hello Sentry! This is a test message");
            
            // Test with levels
            SentrySdk.CaptureMessage("Info level test", SentryLevel.Info);
            SentrySdk.CaptureMessage("Warning level test", SentryLevel.Warning);
            
            // Test with additional data
            SentrySdk.ConfigureScope(scope =>
            {
                scope.SetTag("test-endpoint", "auth-test-sentry");
                scope.SetExtra("timestamp", DateTime.UtcNow);
                scope.User = new SentryUser { Email = "test@cinema-api.com" };
            });
            
            SentrySdk.CaptureMessage("Test message with context data");
            
            // Test exception capture
            try
            {
                throw new Exception("This is a test exception for Sentry");
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
            
            return Ok(new { 
                success = true, 
                message = "Sentry test messages sent! Check your Sentry dashboard.",
                timestamp = DateTime.UtcNow 
            });
        }
    }
}