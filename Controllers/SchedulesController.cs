using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CinemaApi.Interfaces;
using CinemaApi.DTOs.Schedule;
using CinemaApi.DTOs;
using CinemaApi.Responses;

namespace CinemaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SchedulesController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly ILogger<SchedulesController> _logger;

        public SchedulesController(IScheduleService scheduleService, ILogger<SchedulesController> logger)
        {
            _scheduleService = scheduleService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetAllSchedules()
        {
            _logger.LogInformation("Getting all schedules");
            var schedules = await _scheduleService.GetAllSchedulesAsync();
            var response = ApiResponse<IEnumerable<ScheduleDto>>.SuccessResult(schedules, "Schedules retrieved successfully");
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetSchedule(int id)
        {
            _logger.LogInformation("Getting schedule by ID: {ScheduleId}", id);
            var schedule = await _scheduleService.GetScheduleByIdAsync(id);
            
            if (schedule == null)
            {
                _logger.LogWarning("Schedule not found with ID: {ScheduleId}", id);
                throw new KeyNotFoundException($"Schedule with ID {id} not found");
            }

            var response = ApiResponse<ScheduleDto>.SuccessResult(schedule, "Schedule retrieved successfully");
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleDto createScheduleDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating schedule");
                var errorResponse = new ErrorResponse
                {
                    Message = "Validation failed",
                    StatusCode = 400,
                    Path = HttpContext.Request.Path
                };
                return BadRequest(errorResponse);
            }

            _logger.LogInformation("Creating new schedule for Movie ID: {MovieId}, Studio ID: {StudioId}", 
                createScheduleDto.MovieId, createScheduleDto.StudioId);
            var schedule = await _scheduleService.CreateScheduleAsync(createScheduleDto);
            
            var response = ApiResponse<ScheduleDto>.CreatedResult(schedule, "Schedule created successfully");
            return CreatedAtAction(nameof(GetSchedule), new { id = schedule.Id }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] UpdateScheduleDto updateScheduleDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating schedule with ID: {ScheduleId}", id);
                var errorResponse = new ErrorResponse
                {
                    Message = "Validation failed",
                    StatusCode = 400,
                    Path = HttpContext.Request.Path
                };
                return BadRequest(errorResponse);
            }

            _logger.LogInformation("Updating schedule with ID: {ScheduleId}", id);
            var schedule = await _scheduleService.UpdateScheduleAsync(id, updateScheduleDto);
            
            if (schedule == null)
            {
                _logger.LogWarning("Schedule not found for update with ID: {ScheduleId}", id);
                throw new KeyNotFoundException($"Schedule with ID {id} not found");
            }

            var response = ApiResponse<ScheduleDto>.SuccessResult(schedule, "Schedule updated successfully");
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            _logger.LogInformation("Deleting schedule with ID: {ScheduleId}", id);
            
            if (!await _scheduleService.ScheduleExistsAsync(id))
            {
                _logger.LogWarning("Schedule not found for deletion with ID: {ScheduleId}", id);
                throw new KeyNotFoundException($"Schedule with ID {id} not found");
            }

            var result = await _scheduleService.DeleteScheduleAsync(id);
            
            if (result)
            {
                var response = ApiResponse<object>.SuccessResult(new { }, "Schedule deleted successfully");
                return Ok(response);
            }

            var errorResponse = new ErrorResponse
            {
                Message = "Failed to delete schedule",
                StatusCode = 500,
                Path = HttpContext.Request.Path
            };
            return StatusCode(500, errorResponse);
        }
    }
}