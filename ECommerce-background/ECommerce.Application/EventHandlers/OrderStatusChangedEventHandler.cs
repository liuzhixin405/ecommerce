using ECommerce.Core.EventBus;
using ECommerce.Domain.Events;
using ECommerce.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 订单状态变更事件处理器 - 简化版本，只做核心业务逻辑确认
    /// </summary>
    public class OrderStatusChangedEventHandler : IEventHandler<OrderStatusChangedEvent>
    {
        private readonly ILogger<OrderStatusChangedEventHandler> _logger;

        public OrderStatusChangedEventHandler(ILogger<OrderStatusChangedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> HandleAsync(OrderStatusChangedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("OrderStatusChangedEventHandler: Processing status change for order {OrderId}", domainEvent.OrderId);

                // 核心业务逻辑：订单状态变更确认
                await ProcessOrderStatusChangeAsync(domainEvent, cancellationToken);

                _logger.LogInformation("OrderStatusChangedEventHandler: Successfully processed status change for order {OrderId}", domainEvent.OrderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderStatusChangedEventHandler: Error processing status change for order {OrderId}", domainEvent.OrderId);
                return false;
            }
        }

        /// <summary>
        /// 处理订单状态变更的核心业务逻辑
        /// </summary>
        private async Task ProcessOrderStatusChangeAsync(OrderStatusChangedEvent domainEvent, CancellationToken cancellationToken)
        {
            // 1. 记录订单状态变更日志
            _logger.LogInformation("Order {OrderId} status changed from {OldStatus} to {NewStatus} at {Timestamp}", 
                domainEvent.OrderId, domainEvent.OldStatus, domainEvent.NewStatus, domainEvent.OccurredOn);

            // 2. 根据状态变更执行相应的业务逻辑
            switch (domainEvent.NewStatus)
            {
                case OrderStatus.Confirmed:
                    _logger.LogInformation("Order {OrderId} confirmed, preparing for shipment", domainEvent.OrderId);
                    break;
                case OrderStatus.Shipped:
                    _logger.LogInformation("Order {OrderId} shipped with tracking number", domainEvent.OrderId);
                    break;
                case OrderStatus.Delivered:
                    _logger.LogInformation("Order {OrderId} delivered successfully", domainEvent.OrderId);
                    break;
                case OrderStatus.Cancelled:
                    _logger.LogInformation("Order {OrderId} cancelled", domainEvent.OrderId);
                    break;
                default:
                    _logger.LogInformation("Order {OrderId} status updated to {Status}", domainEvent.OrderId, domainEvent.NewStatus);
                    break;
            }

            // 3. 可以在这里添加其他核心业务逻辑
            // 比如：更新库存、发送通知等
            
            // 4. 模拟异步处理
            await Task.Delay(50, cancellationToken);
            
            _logger.LogInformation("Order status change processing completed for order {OrderId}", domainEvent.OrderId);
        }
    }
}
