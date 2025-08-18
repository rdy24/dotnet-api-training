using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CinemaApi.Interfaces;
using CinemaApi.DTOs.Movie;
using CinemaApi.Responses;

namespace CinemaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(IMovieService movieService, ILogger<MoviesController> logger)
        {
            _movieService = movieService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetAllMovies()
        {
            _logger.LogInformation("Getting all movies");
            var movies = await _movieService.GetAllMoviesAsync();
            var response = ApiResponse<IEnumerable<MovieDto>>.SuccessResult(movies, "Movies retrieved successfully");
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetMovie(int id)
        {
            _logger.LogInformation("Getting movie by ID: {MovieId}", id);
            var movie = await _movieService.GetMovieByIdAsync(id);
            
            if (movie == null)
            {
                _logger.LogWarning("Movie not found with ID: {MovieId}", id);
                throw new KeyNotFoundException($"Movie with ID {id} not found");
            }

            var response = ApiResponse<MovieDto>.SuccessResult(movie, "Movie retrieved successfully");
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMovie([FromBody] CreateMovieDto createMovieDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating movie");
                var errorResponse = new ErrorResponse
                {
                    Message = "Validation failed",
                    StatusCode = 400,
                    Path = HttpContext.Request.Path
                };
                return BadRequest(errorResponse);
            }

            _logger.LogInformation("Creating new movie: {MovieTitle}", createMovieDto.Title);
            var movie = await _movieService.CreateMovieAsync(createMovieDto);
            
            var response = ApiResponse<MovieDto>.CreatedResult(movie, "Movie created successfully");
            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] UpdateMovieDto updateMovieDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating movie with ID: {MovieId}", id);
                var errorResponse = new ErrorResponse
                {
                    Message = "Validation failed",
                    StatusCode = 400,
                    Path = HttpContext.Request.Path
                };
                return BadRequest(errorResponse);
            }

            _logger.LogInformation("Updating movie with ID: {MovieId}", id);
            var movie = await _movieService.UpdateMovieAsync(id, updateMovieDto);
            
            if (movie == null)
            {
                _logger.LogWarning("Movie not found for update with ID: {MovieId}", id);
                throw new KeyNotFoundException($"Movie with ID {id} not found");
            }

            var response = ApiResponse<MovieDto>.SuccessResult(movie, "Movie updated successfully");
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            _logger.LogInformation("Deleting movie with ID: {MovieId}", id);
            
            if (!await _movieService.MovieExistsAsync(id))
            {
                _logger.LogWarning("Movie not found for deletion with ID: {MovieId}", id);
                throw new KeyNotFoundException($"Movie with ID {id} not found");
            }

            var result = await _movieService.DeleteMovieAsync(id);
            
            if (result)
            {
                var response = ApiResponse<object>.SuccessResult(new { }, "Movie deleted successfully");
                return Ok(response);
            }

            var errorResponse = new ErrorResponse
            {
                Message = "Failed to delete movie",
                StatusCode = 500,
                Path = HttpContext.Request.Path
            };
            return StatusCode(500, errorResponse);
        }
    }
}