using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ECommerce.API.BackgroundServices
{
    public class OrderExpirationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrderExpirationService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5); // 每5分钟检查一次

        public OrderExpirationService(
            IServiceProvider serviceProvider,
            ILogger<OrderExpirationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Order Expiration Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessExpiredOrders();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing expired orders");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Order Expiration Service is stopping.");
        }

        private async Task ProcessExpiredOrders()
        {
            using var scope = _serviceProvider.CreateScope();
            var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

            try
            {
                await orderService.CancelExpiredOrdersAsync();
                _logger.LogInformation("Processed expired orders successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process expired orders");
            }
        }
    }
}
