using System.Collections.Concurrent;
using CinemaApi.Models;

namespace CinemaApi.Services
{
    public interface ITransactionQueueService
    {
        Task EnqueueTransactionProcessingAsync(int transactionId);
        Task ProcessPendingTransactionsAsync();
    }

    public class TransactionQueueService : BackgroundService, ITransactionQueueService
    {
        private readonly ConcurrentQueue<int> _transactionQueue = new();
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TransactionQueueService> _logger;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public TransactionQueueService(IServiceProvider serviceProvider, ILogger<TransactionQueueService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task EnqueueTransactionProcessingAsync(int transactionId)
        {
            _transactionQueue.Enqueue(transactionId);
            _logger.LogInformation("Transaction {TransactionId} added to processing queue", transactionId);
            return Task.CompletedTask;
        }

        public async Task ProcessPendingTransactionsAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                while (_transactionQueue.TryDequeue(out var transactionId))
                {
                    await ProcessTransactionAsync(transactionId);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Transaction Queue Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPendingTransactionsAsync();
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing transaction queue");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }

            _logger.LogInformation("Transaction Queue Service stopped");
        }

        private async Task ProcessTransactionAsync(int transactionId)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var transactionRepository = scope.ServiceProvider.GetRequiredService<Interfaces.ITransactionRepository>();
                
                var transaction = await transactionRepository.GetByIdAsync(transactionId);
                if (transaction == null)
                {
                    _logger.LogWarning("Transaction {TransactionId} not found for processing", transactionId);
                    return;
                }

                if (transaction.PaymentStatus == PaymentStatus.Pending)
                {
                    _logger.LogInformation("Processing transaction {TransactionId} with amount {Amount}", 
                        transactionId, transaction.Amount);

                    await Task.Delay(1000);

                    transaction.PaymentStatus = PaymentStatus.Success;
                    transaction.PaymentReference = $"REF-{DateTime.UtcNow:yyyyMMddHHmmss}-{transactionId}";

                    await transactionRepository.UpdateAsync(transactionId, transaction);
                    
                    _logger.LogInformation("Transaction {TransactionId} processed successfully", transactionId);
                }
                else
                {
                    _logger.LogInformation("Transaction {TransactionId} already processed with status {Status}", 
                        transactionId, transaction.PaymentStatus);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing transaction {TransactionId}", transactionId);
            }
        }

        public override void Dispose()
        {
            _semaphore?.Dispose();
            base.Dispose();
        }
    }
}