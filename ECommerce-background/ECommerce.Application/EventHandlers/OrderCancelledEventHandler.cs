using ECommerce.Core.EventBus;
using ECommerce.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 订单取消事件处理器 - 简化版本，只做核心业务逻辑确认
    /// </summary>
    public class OrderCancelledEventHandler : IEventHandler<OrderCancelledEvent>
    {
        private readonly ILogger<OrderCancelledEventHandler> _logger;

        public OrderCancelledEventHandler(ILogger<OrderCancelledEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> HandleAsync(OrderCancelledEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("OrderCancelledEventHandler: Processing order cancellation for order {OrderId}", domainEvent.OrderId);

                // 核心业务逻辑：订单取消确认
                await ProcessOrderCancellationAsync(domainEvent, cancellationToken);

                _logger.LogInformation("OrderCancelledEventHandler: Successfully processed order cancellation for order {OrderId}", domainEvent.OrderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderCancelledEventHandler: Error processing order cancellation for order {OrderId}", domainEvent.OrderId);
                return false;
            }
        }

        /// <summary>
        /// 处理订单取消的核心业务逻辑
        /// </summary>
        private async Task ProcessOrderCancellationAsync(OrderCancelledEvent domainEvent, CancellationToken cancellationToken)
        {
            // 1. 记录订单取消日志
            _logger.LogInformation("Order {OrderId} cancelled with reason: {Reason} at {Timestamp}", 
                domainEvent.OrderId, domainEvent.Reason, domainEvent.OccurredOn);

            // 2. 可以在这里添加其他核心业务逻辑
            // 比如：释放库存锁定、处理退款等
            
            // 3. 模拟异步处理
            await Task.Delay(50, cancellationToken);
            
            _logger.LogInformation("Order cancellation processing completed for order {OrderId}", domainEvent.OrderId);
        }
    }
}
