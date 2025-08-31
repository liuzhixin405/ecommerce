using ECommerce.Core.EventBus;
using ECommerce.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 订单创建事件处理器 - 简化版本，只做核心业务逻辑确认
    /// </summary>
    public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedEventHandler> _logger;

        public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> HandleAsync(OrderCreatedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("OrderCreatedEventHandler: Processing order creation for order {OrderId}", domainEvent.OrderId);

                // 核心业务逻辑：订单创建确认
                await ProcessOrderCreationAsync(domainEvent, cancellationToken);

                _logger.LogInformation("OrderCreatedEventHandler: Successfully processed order creation for order {OrderId}", domainEvent.OrderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderCreatedEventHandler: Error processing order creation for order {OrderId}", domainEvent.OrderId);
                return false;
            }
        }

        /// <summary>
        /// 处理订单创建的核心业务逻辑
        /// </summary>
        private async Task ProcessOrderCreationAsync(OrderCreatedEvent domainEvent, CancellationToken cancellationToken)
        {
            // 1. 记录订单创建日志
            _logger.LogInformation("Order {OrderId} created for user {UserId} with total amount {TotalAmount} at {Timestamp}", 
                domainEvent.OrderId, domainEvent.UserId, domainEvent.TotalAmount, domainEvent.OccurredOn);

            // 2. 可以在这里添加其他核心业务逻辑
            // 比如：更新用户订单统计、发送订单确认等
            
            // 3. 模拟异步处理
            await Task.Delay(50, cancellationToken);
            
            _logger.LogInformation("Order creation processing completed for order {OrderId}", domainEvent.OrderId);
        }
    }
}