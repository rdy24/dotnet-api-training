using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CinemaApi.Interfaces;
using CinemaApi.DTOs.Studio;
using CinemaApi.Responses;
using CinemaApi.Attributes;

namespace CinemaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudiosController : ControllerBase
    {
        private readonly IStudioService _studioService;
        private readonly ILogger<StudiosController> _logger;

        public StudiosController(IStudioService studioService, ILogger<StudiosController> logger)
        {
            _studioService = studioService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetAllStudios()
        {
            _logger.LogInformation("Getting all studios");
            var studios = await _studioService.GetAllStudiosAsync();
            var response = ApiResponse<IEnumerable<StudioDto>>.SuccessResult(studios, "Studios retrieved successfully");
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetStudio(int id)
        {
            _logger.LogInformation("Getting studio by ID: {StudioId}", id);
            var studio = await _studioService.GetStudioByIdAsync(id);
            
            if (studio == null)
            {
                _logger.LogWarning("Studio not found with ID: {StudioId}", id);
                throw new KeyNotFoundException($"Studio with ID {id} not found");
            }

            var response = ApiResponse<StudioDto>.SuccessResult(studio, "Studio retrieved successfully");
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStudio([FromBody] CreateStudioDto createStudioDto)
        {
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating studio");
                var errorResponse = new ErrorResponse
                {
                    Message = "Validation failed",
                    StatusCode = 400,
                    Path = HttpContext.Request.Path
                };
                return BadRequest(errorResponse);
            }

            _logger.LogInformation("Creating new studio: {StudioName}", createStudioDto.Name);
            var studio = await _studioService.CreateStudioAsync(createStudioDto);
            
            var response = ApiResponse<StudioDto>.CreatedResult(studio, "Studio created successfully");
            return CreatedAtAction(nameof(GetStudio), new { id = studio.Id }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStudio(int id, [FromBody] UpdateStudioDto updateStudioDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating studio with ID: {StudioId}", id);
                var errorResponse = new ErrorResponse
                {
                    Message = "Validation failed",
                    StatusCode = 400,
                    Path = HttpContext.Request.Path
                };
                return BadRequest(errorResponse);
            }

            _logger.LogInformation("Updating studio with ID: {StudioId}", id);
            var studio = await _studioService.UpdateStudioAsync(id, updateStudioDto);
            
            if (studio == null)
            {
                _logger.LogWarning("Studio not found for update with ID: {StudioId}", id);
                throw new KeyNotFoundException($"Studio with ID {id} not found");
            }

            var response = ApiResponse<StudioDto>.SuccessResult(studio, "Studio updated successfully");
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStudio(int id)
        {
            _logger.LogInformation("Deleting studio with ID: {StudioId}", id);
            
            if (!await _studioService.StudioExistsAsync(id))
            {
                _logger.LogWarning("Studio not found for deletion with ID: {StudioId}", id);
                throw new KeyNotFoundException($"Studio with ID {id} not found");
            }

            var result = await _studioService.DeleteStudioAsync(id);
            
            if (result)
            {
                var response = ApiResponse<object>.SuccessResult(new { }, "Studio deleted successfully");
                return Ok(response);
            }

            var errorResponse = new ErrorResponse
            {
                Message = "Failed to delete studio",
                StatusCode = 500,
                Path = HttpContext.Request.Path
            };
            return StatusCode(500, errorResponse);
        }
    }
}