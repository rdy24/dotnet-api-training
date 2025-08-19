using System.ComponentModel.DataAnnotations;
using CinemaApi.Models;
using CinemaApi.DTOs.Ticket;
using CinemaApi.DTOs.User;

namespace CinemaApi.DTOs.Transaction
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public TicketDto Ticket { get; set; } = new();
        public UserDto User { get; set; } = new();
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? PaymentReference { get; set; }
    }

}