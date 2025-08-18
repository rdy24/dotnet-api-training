using System.ComponentModel.DataAnnotations;

namespace CinemaApi.DTOs.Movie
{

    public class CreateMovieDto
    {
        [Required(ErrorMessage = "Movie title is required")]
        [StringLength(200, ErrorMessage = "Movie title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Genre cannot exceed 50 characters")]
        public string? Genre { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 600, ErrorMessage = "Duration must be between 1 and 600 minutes")]
        public int Duration { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }
    }
}