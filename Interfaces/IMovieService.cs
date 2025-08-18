using CinemaApi.DTOs.Movie;

namespace CinemaApi.Interfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieDto>> GetAllMoviesAsync();
        Task<MovieDto?> GetMovieByIdAsync(int id);
        Task<MovieDto> CreateMovieAsync(CreateMovieDto createMovieDto);
        Task<MovieDto?> UpdateMovieAsync(int id, UpdateMovieDto updateMovieDto);
        Task<bool> DeleteMovieAsync(int id);
        Task<bool> MovieExistsAsync(int id);
    }
}