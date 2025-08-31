using ECommerce.Core.EventBus;
using ECommerce.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 库存更新事件处理器 - 简化版本，只做核心业务逻辑确认
    /// </summary>
    public class InventoryUpdatedEventHandler : IEventHandler<InventoryUpdatedEvent>
    {
        private readonly ILogger<InventoryUpdatedEventHandler> _logger;

        public InventoryUpdatedEventHandler(ILogger<InventoryUpdatedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> HandleAsync(InventoryUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("InventoryUpdatedEventHandler: Processing inventory update for product {ProductId}", domainEvent.ProductId);

                // 核心业务逻辑：库存更新确认
                await ProcessInventoryUpdateAsync(domainEvent, cancellationToken);

                _logger.LogInformation("InventoryUpdatedEventHandler: Successfully processed inventory update for product {ProductId}", domainEvent.ProductId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "InventoryUpdatedEventHandler: Error processing inventory update for product {ProductId}", domainEvent.ProductId);
                return false;
            }
        }

        /// <summary>
        /// 处理库存更新的核心业务逻辑
        /// </summary>
        private async Task ProcessInventoryUpdateAsync(InventoryUpdatedEvent domainEvent, CancellationToken cancellationToken)
        {
            // 1. 记录库存更新日志
            _logger.LogInformation("Product {ProductId} inventory updated from {OldStock} to {NewStock} at {Timestamp}", 
                domainEvent.ProductId, domainEvent.OldStock, domainEvent.NewStock, domainEvent.OccurredOn);

            // 2. 检查低库存警告
            if (domainEvent.NewStock <= 10)
            {
                _logger.LogWarning("Low stock alert: Product {ProductId} has only {Stock} units remaining", 
                    domainEvent.ProductId, domainEvent.NewStock);
            }

            // 3. 可以在这里添加其他核心业务逻辑
            // 比如：更新产品状态、触发补货流程等
            
            // 4. 模拟异步处理
            await Task.Delay(50, cancellationToken);
            
            _logger.LogInformation("Inventory update processing completed for product {ProductId}", domainEvent.ProductId);
        }
    }
}
