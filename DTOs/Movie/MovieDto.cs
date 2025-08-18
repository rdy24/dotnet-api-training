using System.ComponentModel.DataAnnotations;

namespace CinemaApi.DTOs.Movie
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Genre { get; set; }
        public int Duration { get; set; }
        public string? Description { get; set; }
    }
}