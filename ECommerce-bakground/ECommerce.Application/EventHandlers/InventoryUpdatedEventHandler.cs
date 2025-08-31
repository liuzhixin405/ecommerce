using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 库存更新事件处理器
    /// </summary>
    public class InventoryUpdatedEventHandler : IEventHandler<InventoryUpdatedEvent>
    {
        private readonly ILogger<InventoryUpdatedEventHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IStatisticsService _statisticsService;
        private readonly ICacheService _cacheService;
        private readonly INotificationService _notificationService;

        public InventoryUpdatedEventHandler(
            ILogger<InventoryUpdatedEventHandler> logger,
            IEmailService emailService,
            IStatisticsService statisticsService,
            ICacheService cacheService,
            INotificationService notificationService)
        {
            _logger = logger;
            _emailService = emailService;
            _statisticsService = statisticsService;
            _cacheService = cacheService;
            _notificationService = notificationService;
        }

        public async Task<bool> HandleAsync(InventoryUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("InventoryUpdatedEventHandler: Processing inventory update for product {ProductId}", domainEvent.ProductId);

                // 1. 更新库存统计信息
                await _statisticsService.UpdateInventoryStatisticsAsync(domainEvent);

                // 2. 清除相关缓存
                await _cacheService.RemoveByPatternAsync($"product_{domainEvent.ProductId}");
                await _cacheService.RemoveByPatternAsync("products_list");
                await _cacheService.RemoveByPatternAsync("inventory_stats");

                // 3. 检查是否需要发送低库存警告
                if (domainEvent.NewStock <= 10) // 假设库存低于10时发送警告
                {
                    await _emailService.SendLowStockAlertAsync(domainEvent);
                    await _notificationService.SendLowStockNotificationAsync(domainEvent);
                }

                // 4. 更新产品缓存
                await _cacheService.SetAsync($"product_{domainEvent.ProductId}_stock", domainEvent.NewStock, TimeSpan.FromMinutes(30));

                // 5. 记录库存变更日志
                await LogInventoryUpdateAsync(domainEvent, cancellationToken);

                _logger.LogInformation("InventoryUpdatedEventHandler: Successfully processed inventory update for product {ProductId}", domainEvent.ProductId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "InventoryUpdatedEventHandler: Error processing inventory update for product {ProductId}", domainEvent.ProductId);
                return false;
            }
        }

        private async Task LogInventoryUpdateAsync(InventoryUpdatedEvent domainEvent, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("InventoryUpdatedEventHandler: Logging inventory update for product {ProductId} from {OldStock} to {NewStock}", 
                    domainEvent.ProductId, domainEvent.OldStock, domainEvent.NewStock);
                
                // 在实际环境中，这里会记录到库存变更日志表
                await Task.Delay(50, cancellationToken);
                
                _logger.LogInformation("InventoryUpdatedEventHandler: Inventory update logged successfully for product {ProductId}", domainEvent.ProductId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "InventoryUpdatedEventHandler: Failed to log inventory update for product {ProductId}", domainEvent.ProductId);
            }
        }
    }
}
