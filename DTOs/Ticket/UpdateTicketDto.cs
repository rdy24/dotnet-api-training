using System.ComponentModel.DataAnnotations;
using CinemaApi.Models;

namespace CinemaApi.DTOs.Ticket
{

    public class UpdateTicketDto
    {
        [Required(ErrorMessage = "Schedule ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Schedule ID must be a positive number")]
        public int ScheduleId { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive number")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Seat number is required")]
        [StringLength(10, ErrorMessage = "Seat number cannot exceed 10 characters")]
        public string SeatNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required")]
        public TicketStatus Status { get; set; }
    }
}