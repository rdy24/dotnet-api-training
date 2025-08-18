using System.ComponentModel.DataAnnotations;

namespace CinemaApi.DTOs.Studio
{
    public class StudioDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string? Facilities { get; set; }
    }
}