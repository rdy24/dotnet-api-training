using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CinemaApi.DTOs.User;
using CinemaApi.Interfaces;
using CinemaApi.Responses;

namespace CinemaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetUsers()
        {
            _logger.LogInformation("API: Getting all users");
            var users = await _userService.GetAllUsersAsync();
            var response = ApiResponse<IEnumerable<UserDto>>.SuccessResult(users, "Users retrieved successfully");
            _logger.LogInformation("API: Returning {Count} users", users.Count());
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found");
            }
            
            var response = ApiResponse<UserDto>.SuccessResult(user, "User retrieved successfully");
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser(CreateUserDto createUserDto)
        {
            _logger.LogInformation("API: Creating user {Username}", createUserDto.Username);
            var user = await _userService.CreateUserAsync(createUserDto);
            var response = ApiResponse<UserDto>.CreatedResult(user, "User created successfully");
            _logger.LogInformation("API: User created successfully with ID {UserId}", user.Id);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(int id, UpdateUserDto updateUserDto)
        {
            var user = await _userService.UpdateUserAsync(id, updateUserDto);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found");
            }
            
            var response = ApiResponse<UserDto>.SuccessResult(user, "User updated successfully");
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse>> DeleteUser(int id)
        {
            _logger.LogInformation("API: Deleting user with ID {UserId}", id);
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            var response = ApiResponse.SuccessResult("User deleted successfully");
            return Ok(response);
        }
    }
}
