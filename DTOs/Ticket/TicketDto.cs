using System.ComponentModel.DataAnnotations;
using CinemaApi.Models;
using CinemaApi.DTOs.Schedule;
using CinemaApi.DTOs.User;

namespace CinemaApi.DTOs.Ticket
{
    public class TicketDto
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public int UserId { get; set; }
        public ScheduleDto Schedule { get; set; } = new();
        public UserDto User { get; set; } = new();
        public string SeatNumber { get; set; } = string.Empty;
        public TicketStatus Status { get; set; }
        public DateTime BookedAt { get; set; }
    }
}