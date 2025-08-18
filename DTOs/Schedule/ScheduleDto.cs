using System.ComponentModel.DataAnnotations;
using CinemaApi.DTOs.Studio;
using CinemaApi.DTOs.Movie;

namespace CinemaApi.DTOs.Schedule
{
    public class ScheduleDto
    {
        public int Id { get; set; }
        public int StudioId { get; set; }
        public int MovieId { get; set; }
        public StudioDto Studio { get; set; } = new();
        public MovieDto Movie { get; set; } = new();
        public DateTime ShowDateTime { get; set; }
        public decimal TicketPrice { get; set; }
    }
}