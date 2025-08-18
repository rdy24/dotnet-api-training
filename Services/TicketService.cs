using CinemaApi.DTOs.Ticket;
using CinemaApi.DTOs.Schedule;
using CinemaApi.DTOs.User;
using CinemaApi.DTOs.Movie;
using CinemaApi.DTOs.Studio;
using CinemaApi.Interfaces;
using CinemaApi.Models;

namespace CinemaApi.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILogger<TicketService> _logger;

        public TicketService(ITicketRepository ticketRepository, ILogger<TicketService> logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<TicketDto>> GetAllTicketsAsync()
        {
            var tickets = await _ticketRepository.GetAllAsync();
            return tickets.Select(ticket => new TicketDto
            {
                Id = ticket.Id,
                ScheduleId = ticket.ScheduleId,
                UserId = ticket.UserId,
                SeatNumber = ticket.SeatNumber,
                Status = ticket.Status,
                BookedAt = ticket.BookedAt,
                Schedule = new ScheduleDto
                {
                    Id = ticket.Schedule.Id,
                    StudioId = ticket.Schedule.StudioId,
                    MovieId = ticket.Schedule.MovieId,
                    ShowDateTime = ticket.Schedule.ShowDateTime,
                    TicketPrice = ticket.Schedule.TicketPrice,
                    Studio = new StudioDto
                    {
                        Id = ticket.Schedule.Studio.Id,
                        Name = ticket.Schedule.Studio.Name,
                        Capacity = ticket.Schedule.Studio.Capacity
                    },
                    Movie = new MovieDto
                    {
                        Id = ticket.Schedule.Movie.Id,
                        Title = ticket.Schedule.Movie.Title,
                        Genre = ticket.Schedule.Movie.Genre,
                        Duration = ticket.Schedule.Movie.Duration,
                        Description = ticket.Schedule.Movie.Description
                    }
                },
                User = new UserDto
                {
                    Id = ticket.User.Id,
                    Username = ticket.User.Username,
                    Email = ticket.User.Email,
                    Role = ticket.User.Role,
                    CreatedAt = ticket.User.CreatedAt
                }
            });
        }

        public async Task<TicketDto?> GetTicketByIdAsync(int id)
        {
            _logger.LogInformation("Getting ticket by ID: {TicketId}", id);
            var ticket = await _ticketRepository.GetByIdAsync(id);
            
            if (ticket == null)
            {
                _logger.LogWarning("Ticket not found with ID: {TicketId}", id);
                return null;
            }

            _logger.LogInformation("Ticket found: {SeatNumber} for Schedule {ScheduleId} (ID: {TicketId})", 
                ticket.SeatNumber, ticket.ScheduleId, ticket.Id);

            return new TicketDto
            {
                Id = ticket.Id,
                ScheduleId = ticket.ScheduleId,
                UserId = ticket.UserId,
                SeatNumber = ticket.SeatNumber,
                Status = ticket.Status,
                BookedAt = ticket.BookedAt,
                Schedule = new ScheduleDto
                {
                    Id = ticket.Schedule.Id,
                    StudioId = ticket.Schedule.StudioId,
                    MovieId = ticket.Schedule.MovieId,
                    ShowDateTime = ticket.Schedule.ShowDateTime,
                    TicketPrice = ticket.Schedule.TicketPrice,
                    Studio = new StudioDto
                    {
                        Id = ticket.Schedule.Studio.Id,
                        Name = ticket.Schedule.Studio.Name,
                        Capacity = ticket.Schedule.Studio.Capacity
                    },
                    Movie = new MovieDto
                    {
                        Id = ticket.Schedule.Movie.Id,
                        Title = ticket.Schedule.Movie.Title,
                        Genre = ticket.Schedule.Movie.Genre,
                        Duration = ticket.Schedule.Movie.Duration,
                        Description = ticket.Schedule.Movie.Description
                    }
                },
                User = new UserDto
                {
                    Id = ticket.User.Id,
                    Username = ticket.User.Username,
                    Email = ticket.User.Email,
                    Role = ticket.User.Role,
                    CreatedAt = ticket.User.CreatedAt
                }
            };
        }

        public async Task<TicketDto> CreateTicketAsync(CreateTicketDto createTicketDto)
        {
            _logger.LogInformation("Creating new ticket: Seat {SeatNumber} for Schedule {ScheduleId} and User {UserId}", 
                createTicketDto.SeatNumber, createTicketDto.ScheduleId, createTicketDto.UserId);

            // Business logic validation
            if (!await _ticketRepository.ScheduleExistsAsync(createTicketDto.ScheduleId))
            {
                throw new InvalidOperationException("Schedule does not exist");
            }

            if (!await _ticketRepository.UserExistsAsync(createTicketDto.UserId))
            {
                throw new InvalidOperationException("User does not exist");
            }

            if (!await _ticketRepository.SeatAvailableAsync(createTicketDto.ScheduleId, createTicketDto.SeatNumber))
            {
                throw new InvalidOperationException("Seat is not available for this schedule");
            }

            var ticket = new Ticket
            {
                ScheduleId = createTicketDto.ScheduleId,
                UserId = createTicketDto.UserId,
                SeatNumber = createTicketDto.SeatNumber,
                Status = createTicketDto.Status
            };

            var createdTicket = await _ticketRepository.CreateAsync(ticket);
            
            _logger.LogInformation("Ticket created successfully with ID {TicketId}", createdTicket.Id);

            return new TicketDto
            {
                Id = createdTicket.Id,
                ScheduleId = createdTicket.ScheduleId,
                UserId = createdTicket.UserId,
                SeatNumber = createdTicket.SeatNumber,
                Status = createdTicket.Status,
                BookedAt = createdTicket.BookedAt,
                Schedule = new ScheduleDto
                {
                    Id = createdTicket.Schedule.Id,
                    StudioId = createdTicket.Schedule.StudioId,
                    MovieId = createdTicket.Schedule.MovieId,
                    ShowDateTime = createdTicket.Schedule.ShowDateTime,
                    TicketPrice = createdTicket.Schedule.TicketPrice,
                    Studio = new StudioDto
                    {
                        Id = createdTicket.Schedule.Studio.Id,
                        Name = createdTicket.Schedule.Studio.Name,
                        Capacity = createdTicket.Schedule.Studio.Capacity
                    },
                    Movie = new MovieDto
                    {
                        Id = createdTicket.Schedule.Movie.Id,
                        Title = createdTicket.Schedule.Movie.Title,
                        Genre = createdTicket.Schedule.Movie.Genre,
                        Duration = createdTicket.Schedule.Movie.Duration,
                        Description = createdTicket.Schedule.Movie.Description
                    }
                },
                User = new UserDto
                {
                    Id = createdTicket.User.Id,
                    Username = createdTicket.User.Username,
                    Email = createdTicket.User.Email,
                    Role = createdTicket.User.Role,
                    CreatedAt = createdTicket.User.CreatedAt
                }
            };
        }

        public async Task<TicketDto?> UpdateTicketAsync(int id, UpdateTicketDto updateTicketDto)
        {
            _logger.LogInformation("Updating ticket with ID {TicketId}: Seat {SeatNumber} for Schedule {ScheduleId}", 
                id, updateTicketDto.SeatNumber, updateTicketDto.ScheduleId);

            // Check if ticket exists
            if (!await _ticketRepository.ExistsAsync(id))
            {
                return null;
            }

            // Business logic validation
            if (!await _ticketRepository.ScheduleExistsAsync(updateTicketDto.ScheduleId))
            {
                throw new InvalidOperationException("Schedule does not exist");
            }

            if (!await _ticketRepository.UserExistsAsync(updateTicketDto.UserId))
            {
                throw new InvalidOperationException("User does not exist");
            }

            // Check seat availability (excluding current ticket)
            if (!await _ticketRepository.SeatAvailableAsync(updateTicketDto.ScheduleId, updateTicketDto.SeatNumber, id))
            {
                throw new InvalidOperationException("Seat is not available for this schedule");
            }

            var ticket = new Ticket
            {
                ScheduleId = updateTicketDto.ScheduleId,
                UserId = updateTicketDto.UserId,
                SeatNumber = updateTicketDto.SeatNumber,
                Status = updateTicketDto.Status
            };

            var updatedTicket = await _ticketRepository.UpdateAsync(id, ticket);
            if (updatedTicket == null)
            {
                return null;
            }

            _logger.LogInformation("Ticket updated successfully with ID {TicketId}", updatedTicket.Id);

            return new TicketDto
            {
                Id = updatedTicket.Id,
                ScheduleId = updatedTicket.ScheduleId,
                UserId = updatedTicket.UserId,
                SeatNumber = updatedTicket.SeatNumber,
                Status = updatedTicket.Status,
                BookedAt = updatedTicket.BookedAt,
                Schedule = new ScheduleDto
                {
                    Id = updatedTicket.Schedule.Id,
                    StudioId = updatedTicket.Schedule.StudioId,
                    MovieId = updatedTicket.Schedule.MovieId,
                    ShowDateTime = updatedTicket.Schedule.ShowDateTime,
                    TicketPrice = updatedTicket.Schedule.TicketPrice,
                    Studio = new StudioDto
                    {
                        Id = updatedTicket.Schedule.Studio.Id,
                        Name = updatedTicket.Schedule.Studio.Name,
                        Capacity = updatedTicket.Schedule.Studio.Capacity
                    },
                    Movie = new MovieDto
                    {
                        Id = updatedTicket.Schedule.Movie.Id,
                        Title = updatedTicket.Schedule.Movie.Title,
                        Genre = updatedTicket.Schedule.Movie.Genre,
                        Duration = updatedTicket.Schedule.Movie.Duration,
                        Description = updatedTicket.Schedule.Movie.Description
                    }
                },
                User = new UserDto
                {
                    Id = updatedTicket.User.Id,
                    Username = updatedTicket.User.Username,
                    Email = updatedTicket.User.Email,
                    Role = updatedTicket.User.Role,
                    CreatedAt = updatedTicket.User.CreatedAt
                }
            };
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            _logger.LogInformation("Deleting ticket with ID {TicketId}", id);
            var result = await _ticketRepository.DeleteAsync(id);
            
            if (result)
                _logger.LogInformation("Ticket deleted successfully with ID {TicketId}", id);
            else
                _logger.LogWarning("Failed to delete ticket with ID {TicketId}", id);
                
            return result;
        }

        public async Task<bool> TicketExistsAsync(int id)
        {
            return await _ticketRepository.ExistsAsync(id);
        }
    }
}
