using ECommerce.Core.EventBus;
using ECommerce.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 订单支付事件处理器 - 简化版本，只做核心业务逻辑确认
    /// </summary>
    public class OrderPaidEventHandler : IEventHandler<OrderPaidEvent>
    {
        private readonly ILogger<OrderPaidEventHandler> _logger;

        public OrderPaidEventHandler(ILogger<OrderPaidEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> HandleAsync(OrderPaidEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("OrderPaidEventHandler: Processing order payment for order {OrderId}", domainEvent.OrderId);

                // 核心业务逻辑：订单支付确认
                await ProcessOrderPaymentAsync(domainEvent, cancellationToken);

                _logger.LogInformation("OrderPaidEventHandler: Successfully processed order payment for order {OrderId}", domainEvent.OrderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderPaidEventHandler: Error processing order payment for order {OrderId}", domainEvent.OrderId);
                return false;
            }
        }

        /// <summary>
        /// 处理订单支付的核心业务逻辑
        /// </summary>
        private async Task ProcessOrderPaymentAsync(OrderPaidEvent domainEvent, CancellationToken cancellationToken)
        {
            // 1. 记录订单支付日志
            _logger.LogInformation("Order {OrderId} paid with amount {Amount} at {Timestamp}", 
                domainEvent.OrderId, domainEvent.Amount, domainEvent.OccurredOn);

            // 2. 可以在这里添加其他核心业务逻辑
            // 比如：更新订单状态、触发发货流程等
            
            // 3. 模拟异步处理
            await Task.Delay(50, cancellationToken);
            
            _logger.LogInformation("Order payment processing completed for order {OrderId}", domainEvent.OrderId);
        }
    }
}

