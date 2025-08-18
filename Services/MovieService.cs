using CinemaApi.DTOs.Movie;
using CinemaApi.Interfaces;
using CinemaApi.Models;

namespace CinemaApi.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ILogger<MovieService> _logger;

        public MovieService(IMovieRepository movieRepository, ILogger<MovieService> logger)
        {
            _movieRepository = movieRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<MovieDto>> GetAllMoviesAsync()
        {
            var movies = await _movieRepository.GetAllAsync();
            
            var movieDtos = movies.Select(m => new MovieDto
            {
                Id = m.Id,
                Title = m.Title,
                Genre = m.Genre,
                Duration = m.Duration,
                Description = m.Description
            });

            return movieDtos;
        }

        public async Task<MovieDto?> GetMovieByIdAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            
            if (movie == null)
            {
                return null;
            }

            return new MovieDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Genre = movie.Genre,
                Duration = movie.Duration,
                Description = movie.Description
            };
        }

        public async Task<MovieDto> CreateMovieAsync(CreateMovieDto createMovieDto)
        {
            _logger.LogInformation("Creating new movie: {MovieTitle}", createMovieDto.Title);

            var movie = new Movie
            {
                Title = createMovieDto.Title,
                Genre = createMovieDto.Genre,
                Duration = createMovieDto.Duration,
                Description = createMovieDto.Description
            };

            var createdMovie = await _movieRepository.CreateAsync(movie);
            _logger.LogInformation("Movie created successfully: {MovieTitle} (ID: {MovieId})", 
                createdMovie.Title, createdMovie.Id);

            return new MovieDto
            {
                Id = createdMovie.Id,
                Title = createdMovie.Title,
                Genre = createdMovie.Genre,
                Duration = createdMovie.Duration,
                Description = createdMovie.Description
            };
        }

        public async Task<MovieDto?> UpdateMovieAsync(int id, UpdateMovieDto updateMovieDto)
        {
            _logger.LogInformation("Updating movie with ID: {MovieId}", id);

            if (!await _movieRepository.ExistsAsync(id))
            {
                return null;
            }

            var movieToUpdate = new Movie
            {
                Title = updateMovieDto.Title,
                Genre = updateMovieDto.Genre,
                Duration = updateMovieDto.Duration,
                Description = updateMovieDto.Description
            };

            var updatedMovie = await _movieRepository.UpdateAsync(id, movieToUpdate);
            if (updatedMovie != null)
            {
                _logger.LogInformation("Movie updated successfully: {MovieTitle} (ID: {MovieId})", 
                    updatedMovie.Title, updatedMovie.Id);
                
                return new MovieDto
                {
                    Id = updatedMovie.Id,
                    Title = updatedMovie.Title,
                    Genre = updatedMovie.Genre,
                    Duration = updatedMovie.Duration,
                    Description = updatedMovie.Description
                };
            }

            return null;
        }

        public async Task<bool> DeleteMovieAsync(int id)
        {
            if (!await _movieRepository.ExistsAsync(id))
            {
                return false;
            }

            var result = await _movieRepository.DeleteAsync(id);
            return result;
        }

        public async Task<bool> MovieExistsAsync(int id)
        {
            return await _movieRepository.ExistsAsync(id);
        }
    }
}