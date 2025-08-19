using CinemaApi.Models;

namespace CinemaApi.Services
{
    public interface ITransactionSchedulerService
    {
        Task ScheduleTransactionCleanupAsync();
    }

    public class TransactionSchedulerService : BackgroundService, ITransactionSchedulerService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TransactionSchedulerService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(6);

        public TransactionSchedulerService(IServiceProvider serviceProvider, ILogger<TransactionSchedulerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task ScheduleTransactionCleanupAsync()
        {
            _logger.LogInformation("Manual transaction cleanup scheduled");
            _ = Task.Run(async () => await CleanupExpiredTransactionsAsync());
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Transaction Scheduler Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredTransactionsAsync();
                    await Task.Delay(_cleanupInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in transaction scheduler");
                    await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
                }
            }

            _logger.LogInformation("Transaction Scheduler Service stopped");
        }

        private async Task CleanupExpiredTransactionsAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var transactionRepository = scope.ServiceProvider.GetRequiredService<Interfaces.ITransactionRepository>();
                
                _logger.LogInformation("Starting cleanup of expired transactions");

                var allTransactions = await transactionRepository.GetAllAsync();
                var expiredTransactions = allTransactions
                    .Where(t => t.PaymentStatus == PaymentStatus.Pending && 
                               t.TransactionDate < DateTime.UtcNow.AddHours(-24))
                    .ToList();

                _logger.LogInformation("Found {Count} expired pending transactions", expiredTransactions.Count);

                foreach (var transaction in expiredTransactions)
                {
                    transaction.PaymentStatus = PaymentStatus.Failed;
                    await transactionRepository.UpdateAsync(transaction.Id, transaction);
                    _logger.LogInformation("Marked transaction {TransactionId} as failed due to expiration", transaction.Id);
                }

                if (expiredTransactions.Any())
                {
                    _logger.LogInformation("Completed cleanup of {Count} expired transactions", expiredTransactions.Count);
                }
                else
                {
                    _logger.LogInformation("No expired transactions found for cleanup");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during transaction cleanup");
            }
        }
    }
}