using CinemaApi.DTOs.Auth;
using CinemaApi.Responses;

namespace CinemaApi.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginDto loginDto);
    }
}