using CinemaApi.DTOs.User;

namespace CinemaApi.Responses
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public UserDto User { get; set; } = new();
    }
}