using CinemaApi.DTOs.Transaction;
using CinemaApi.DTOs.Ticket;
using CinemaApi.DTOs.User;
using CinemaApi.DTOs.Schedule;
using CinemaApi.DTOs.Movie;
using CinemaApi.DTOs.Studio;
using CinemaApi.Interfaces;
using CinemaApi.Models;

namespace CinemaApi.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(ITransactionRepository transactionRepository, ILogger<TransactionService> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            return transactions.Select(transaction => MapToDto(transaction));
        }

        public async Task<TransactionDto?> GetTransactionByIdAsync(int id)
        {
            _logger.LogInformation("Getting transaction by ID: {TransactionId}", id);
            var transaction = await _transactionRepository.GetByIdAsync(id);
            
            if (transaction == null)
            {
                _logger.LogWarning("Transaction not found with ID: {TransactionId}", id);
                return null;
            }

            _logger.LogInformation("Transaction found: Amount {Amount} for Ticket {TicketId} (ID: {TransactionId})", 
                transaction.Amount, transaction.TicketId, transaction.Id);

            return MapToDto(transaction);
        }

        public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto createTransactionDto)
        {
            _logger.LogInformation("Creating transaction for Ticket {TicketId}, User {UserId}, Amount {Amount}", 
                createTransactionDto.TicketId, createTransactionDto.UserId, createTransactionDto.Amount);

            if (!await _transactionRepository.TicketExistsAsync(createTransactionDto.TicketId))
            {
                throw new ArgumentException($"Ticket with ID {createTransactionDto.TicketId} does not exist");
            }

            if (!await _transactionRepository.UserExistsAsync(createTransactionDto.UserId))
            {
                throw new ArgumentException($"User with ID {createTransactionDto.UserId} does not exist");
            }

            if (await _transactionRepository.TicketAlreadyPaidAsync(createTransactionDto.TicketId))
            {
                throw new InvalidOperationException($"Ticket {createTransactionDto.TicketId} has already been paid for");
            }

            var transaction = new Transaction
            {
                TicketId = createTransactionDto.TicketId,
                UserId = createTransactionDto.UserId,
                Amount = createTransactionDto.Amount,
                PaymentMethod = createTransactionDto.PaymentMethod,
                PaymentStatus = createTransactionDto.PaymentStatus,
                PaymentReference = createTransactionDto.PaymentReference
            };

            var createdTransaction = await _transactionRepository.CreateAsync(transaction);
            _logger.LogInformation("Transaction created successfully with ID: {TransactionId}", createdTransaction.Id);
            
            return MapToDto(createdTransaction);
        }

        public async Task<TransactionDto?> UpdateTransactionAsync(int id, UpdateTransactionDto updateTransactionDto)
        {
            _logger.LogInformation("Updating transaction ID: {TransactionId}", id);

            if (!await _transactionRepository.ExistsAsync(id))
            {
                return null;
            }

            if (!await _transactionRepository.TicketExistsAsync(updateTransactionDto.TicketId))
            {
                throw new ArgumentException($"Ticket with ID {updateTransactionDto.TicketId} does not exist");
            }

            if (!await _transactionRepository.UserExistsAsync(updateTransactionDto.UserId))
            {
                throw new ArgumentException($"User with ID {updateTransactionDto.UserId} does not exist");
            }

            if (await _transactionRepository.TicketAlreadyPaidAsync(updateTransactionDto.TicketId, id))
            {
                throw new InvalidOperationException($"Ticket {updateTransactionDto.TicketId} has already been paid for by another transaction");
            }

            var transaction = new Transaction
            {
                Id = id,
                TicketId = updateTransactionDto.TicketId,
                UserId = updateTransactionDto.UserId,
                Amount = updateTransactionDto.Amount,
                PaymentMethod = updateTransactionDto.PaymentMethod,
                PaymentStatus = updateTransactionDto.PaymentStatus,
                PaymentReference = updateTransactionDto.PaymentReference
            };

            var updatedTransaction = await _transactionRepository.UpdateAsync(id, transaction);
            if (updatedTransaction != null)
            {
                _logger.LogInformation("Transaction updated successfully: {TransactionId}", id);
                return MapToDto(updatedTransaction);
            }

            return null;
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            _logger.LogInformation("Deleting transaction ID: {TransactionId}", id);
            var result = await _transactionRepository.DeleteAsync(id);
            
            if (result)
            {
                _logger.LogInformation("Transaction deleted successfully: {TransactionId}", id);
            }
            else
            {
                _logger.LogWarning("Failed to delete transaction: {TransactionId}", id);
            }

            return result;
        }

        public async Task<bool> TransactionExistsAsync(int id)
        {
            return await _transactionRepository.ExistsAsync(id);
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByUserIdAsync(int userId)
        {
            _logger.LogInformation("Getting transactions for User ID: {UserId}", userId);
            var transactions = await _transactionRepository.GetByUserIdAsync(userId);
            return transactions.Select(transaction => MapToDto(transaction));
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByTicketIdAsync(int ticketId)
        {
            _logger.LogInformation("Getting transactions for Ticket ID: {TicketId}", ticketId);
            var transactions = await _transactionRepository.GetByTicketIdAsync(ticketId);
            return transactions.Select(transaction => MapToDto(transaction));
        }

        private static TransactionDto MapToDto(Transaction transaction)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                TicketId = transaction.TicketId,
                UserId = transaction.UserId,
                Amount = transaction.Amount,
                PaymentMethod = transaction.PaymentMethod,
                PaymentStatus = transaction.PaymentStatus,
                TransactionDate = transaction.TransactionDate,
                PaymentReference = transaction.PaymentReference,
                Ticket = new TicketDto
                {
                    Id = transaction.Ticket.Id,
                    ScheduleId = transaction.Ticket.ScheduleId,
                    UserId = transaction.Ticket.UserId,
                    SeatNumber = transaction.Ticket.SeatNumber,
                    Status = transaction.Ticket.Status,
                    BookedAt = transaction.Ticket.BookedAt,
                    Schedule = new ScheduleDto
                    {
                        Id = transaction.Ticket.Schedule.Id,
                        StudioId = transaction.Ticket.Schedule.StudioId,
                        MovieId = transaction.Ticket.Schedule.MovieId,
                        ShowDateTime = transaction.Ticket.Schedule.ShowDateTime,
                        TicketPrice = transaction.Ticket.Schedule.TicketPrice,
                        Studio = new StudioDto
                        {
                            Id = transaction.Ticket.Schedule.Studio.Id,
                            Name = transaction.Ticket.Schedule.Studio.Name,
                            Capacity = transaction.Ticket.Schedule.Studio.Capacity
                        },
                        Movie = new MovieDto
                        {
                            Id = transaction.Ticket.Schedule.Movie.Id,
                            Title = transaction.Ticket.Schedule.Movie.Title,
                            Genre = transaction.Ticket.Schedule.Movie.Genre,
                            Duration = transaction.Ticket.Schedule.Movie.Duration,
                            Description = transaction.Ticket.Schedule.Movie.Description
                        }
                    },
                    User = new UserDto
                    {
                        Id = transaction.Ticket.User.Id,
                        Username = transaction.Ticket.User.Username,
                        Email = transaction.Ticket.User.Email,
                        Role = transaction.Ticket.User.Role,
                        CreatedAt = transaction.Ticket.User.CreatedAt
                    }
                },
                User = new UserDto
                {
                    Id = transaction.User.Id,
                    Username = transaction.User.Username,
                    Email = transaction.User.Email,
                    Role = transaction.User.Role,
                    CreatedAt = transaction.User.CreatedAt
                }
            };
        }
    }
}