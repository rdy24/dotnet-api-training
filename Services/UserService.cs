using CinemaApi.DTOs.User;
using CinemaApi.Interfaces;

namespace CinemaApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users;
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            _logger.LogInformation("Getting user by ID: {UserId}", id);
            var user = await _userRepository.GetUserByIdAsync(id);
            
            if (user == null)
                _logger.LogWarning("User not found with ID: {UserId}", id);
            else
                _logger.LogInformation("User found: {Username} (ID: {UserId})", user.Username, user.Id);
                
            return user;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            _logger.LogInformation("Creating new user: {Username} ({Email})", createUserDto.Username, createUserDto.Email);

            // Business logic validation
            if (await _userRepository.UsernameExistsAsync(createUserDto.Username))
            {
                throw new InvalidOperationException("Username already exists");
            }

            if (await _userRepository.EmailExistsAsync(createUserDto.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            var user = await _userRepository.CreateUserAsync(createUserDto);
            _logger.LogInformation("User created successfully: {Username} (ID: {UserId})", user.Username, user.Id);
            
            return user;
        }

        public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            // Business logic validation
            if (!await _userRepository.UserExistsAsync(id))
            {
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            // Check if email already exists (if updating email)
            if (!string.IsNullOrEmpty(updateUserDto.Email))
            {
                var existingUser = await _userRepository.GetUserByEmailAsync(updateUserDto.Email);
                if (existingUser != null && existingUser.Id != id)
                {
                    throw new InvalidOperationException("Email already exists");
                }
            }

            return await _userRepository.UpdateUserAsync(id, updateUserDto);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            // Business logic validation
            if (!await _userRepository.UserExistsAsync(id))
            {
                return false;
            }


            return await _userRepository.DeleteUserAsync(id);
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await _userRepository.UserExistsAsync(id);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _userRepository.UsernameExistsAsync(username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userRepository.EmailExistsAsync(email);
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }
    }
}