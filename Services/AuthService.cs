using CinemaApi.DTOs.Auth;
using CinemaApi.DTOs.User;
using CinemaApi.Interfaces;
using CinemaApi.Responses;

namespace CinemaApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository authRepository, ILogger<AuthService> logger)
        {
            _authRepository = authRepository;
            _logger = logger;
        }

        public async Task<AuthResponse?> LoginAsync(LoginDto loginDto)
        {
            _logger.LogInformation("Attempting login for username: {Username}", loginDto.Username);

            var user = await _authRepository.AuthenticateAsync(loginDto.Username, loginDto.Password);
            if (user == null)
            {
                _logger.LogWarning("Login failed for username: {Username}", loginDto.Username);
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            var token = _authRepository.GenerateJwtToken(user);
            var expiresAt = DateTime.UtcNow.AddHours(24); // Default 24 hours

            var authResponse = new AuthResponse
            {
                Token = token,
                Expires = expiresAt,
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Username = user.Username,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive
                }
            };

            _logger.LogInformation("Login successful for user: {Username} (ID: {UserId})", user.Username, user.Id);
            return authResponse;
        }
    }
}