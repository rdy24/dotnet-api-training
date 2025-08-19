using CinemaApi.DTOs.Transaction;

namespace CinemaApi.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync();
        Task<TransactionDto?> GetTransactionByIdAsync(int id);
        Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto createTransactionDto);
        Task<TransactionDto?> UpdateTransactionAsync(int id, UpdateTransactionDto updateTransactionDto);
        Task<bool> DeleteTransactionAsync(int id);
        Task<bool> TransactionExistsAsync(int id);
        Task<IEnumerable<TransactionDto>> GetTransactionsByUserIdAsync(int userId);
        Task<IEnumerable<TransactionDto>> GetTransactionsByTicketIdAsync(int ticketId);
    }
}