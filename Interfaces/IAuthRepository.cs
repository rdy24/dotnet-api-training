using CinemaApi.Models;

namespace CinemaApi.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> AuthenticateAsync(string username, string password);
        string GenerateJwtToken(User user);
    }
}