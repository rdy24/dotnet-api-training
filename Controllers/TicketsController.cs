using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CinemaApi.DTOs.Ticket;
using CinemaApi.Interfaces;
using CinemaApi.Responses;

namespace CinemaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(ITicketService ticketService, ILogger<TicketsController> logger)
        {
            _ticketService = ticketService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TicketDto>>>> GetTickets()
        {
            _logger.LogInformation("API: Getting all tickets");
            var tickets = await _ticketService.GetAllTicketsAsync();
            var response = ApiResponse<IEnumerable<TicketDto>>.SuccessResult(tickets, "Tickets retrieved successfully");
            _logger.LogInformation("API: Returning {Count} tickets", tickets.Count());
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<ApiResponse<TicketDto>>> GetTicket(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
            {
                throw new KeyNotFoundException($"Ticket with ID {id} not found");
            }
            
            var response = ApiResponse<TicketDto>.SuccessResult(ticket, "Ticket retrieved successfully");
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<ApiResponse<TicketDto>>> CreateTicket(CreateTicketDto createTicketDto)
        {
            _logger.LogInformation("API: Creating ticket for seat {SeatNumber} in schedule {ScheduleId}", 
                createTicketDto.SeatNumber, createTicketDto.ScheduleId);
            var ticket = await _ticketService.CreateTicketAsync(createTicketDto);
            var response = ApiResponse<TicketDto>.CreatedResult(ticket, "Ticket created successfully");
            _logger.LogInformation("API: Ticket created successfully with ID {TicketId}", ticket.Id);
            return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<TicketDto>>> UpdateTicket(int id, UpdateTicketDto updateTicketDto)
        {
            _logger.LogInformation("API: Updating ticket with ID {TicketId}", id);
            var ticket = await _ticketService.UpdateTicketAsync(id, updateTicketDto);
            if (ticket == null)
            {
                throw new KeyNotFoundException($"Ticket with ID {id} not found");
            }
            
            var response = ApiResponse<TicketDto>.SuccessResult(ticket, "Ticket updated successfully");
            _logger.LogInformation("API: Ticket updated successfully with ID {TicketId}", ticket.Id);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse>> DeleteTicket(int id)
        {
            _logger.LogInformation("API: Deleting ticket with ID {TicketId}", id);
            var result = await _ticketService.DeleteTicketAsync(id);
            if (!result)
            {
                throw new KeyNotFoundException($"Ticket with ID {id} not found");
            }
            
            var response = ApiResponse.SuccessResult("Ticket deleted successfully");
            _logger.LogInformation("API: Ticket deleted successfully with ID {TicketId}", id);
            return Ok(response);
        }
    }
}
