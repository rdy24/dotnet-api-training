using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CinemaApi.DTOs.Transaction;
using CinemaApi.Interfaces;
using CinemaApi.Responses;

namespace CinemaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TransactionDto>>>> GetTransactions()
        {
            _logger.LogInformation("API: Getting all transactions");
            var transactions = await _transactionService.GetAllTransactionsAsync();
            var response = ApiResponse<IEnumerable<TransactionDto>>.SuccessResult(transactions, "Transactions retrieved successfully");
            _logger.LogInformation("API: Returning {Count} transactions", transactions.Count());
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<ApiResponse<TransactionDto>>> GetTransaction(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction == null)
            {
                throw new KeyNotFoundException($"Transaction with ID {id} not found");
            }
            
            var response = ApiResponse<TransactionDto>.SuccessResult(transaction, "Transaction retrieved successfully");
            return Ok(response);
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TransactionDto>>>> GetTransactionsByUserId(int userId)
        {
            _logger.LogInformation("API: Getting transactions for user {UserId}", userId);
            var transactions = await _transactionService.GetTransactionsByUserIdAsync(userId);
            var response = ApiResponse<IEnumerable<TransactionDto>>.SuccessResult(transactions, "User transactions retrieved successfully");
            _logger.LogInformation("API: Returning {Count} transactions for user {UserId}", transactions.Count(), userId);
            return Ok(response);
        }

        [HttpGet("ticket/{ticketId}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TransactionDto>>>> GetTransactionsByTicketId(int ticketId)
        {
            _logger.LogInformation("API: Getting transactions for ticket {TicketId}", ticketId);
            var transactions = await _transactionService.GetTransactionsByTicketIdAsync(ticketId);
            var response = ApiResponse<IEnumerable<TransactionDto>>.SuccessResult(transactions, "Ticket transactions retrieved successfully");
            _logger.LogInformation("API: Returning {Count} transactions for ticket {TicketId}", transactions.Count(), ticketId);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<ApiResponse<TransactionDto>>> CreateTransaction(CreateTransactionDto createTransactionDto)
        {
            _logger.LogInformation("API: Creating transaction for ticket {TicketId}", createTransactionDto.TicketId);
            
            var transaction = await _transactionService.CreateTransactionAsync(createTransactionDto);
            var response = ApiResponse<TransactionDto>.SuccessResult(transaction, "Transaction created successfully");
            
            _logger.LogInformation("API: Transaction created with ID {TransactionId}", transaction.Id);
            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<TransactionDto>>> UpdateTransaction(int id, UpdateTransactionDto updateTransactionDto)
        {
            _logger.LogInformation("API: Updating transaction {TransactionId}", id);
            
            var transaction = await _transactionService.UpdateTransactionAsync(id, updateTransactionDto);
            if (transaction == null)
            {
                throw new KeyNotFoundException($"Transaction with ID {id} not found");
            }
            
            var response = ApiResponse<TransactionDto>.SuccessResult(transaction, "Transaction updated successfully");
            _logger.LogInformation("API: Transaction {TransactionId} updated successfully", id);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteTransaction(int id)
        {
            _logger.LogInformation("API: Deleting transaction {TransactionId}", id);
            
            var result = await _transactionService.DeleteTransactionAsync(id);
            if (!result)
            {
                throw new KeyNotFoundException($"Transaction with ID {id} not found");
            }
            
            var response = ApiResponse<object>.SuccessResult(new { }, "Transaction deleted successfully");
            _logger.LogInformation("API: Transaction {TransactionId} deleted successfully", id);
            return Ok(response);
        }
    }
}