using ECommerce.Core.EventBus;
using ECommerce.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 支付处理事件处理器 - 简化版本，只做核心业务逻辑确认
    /// </summary>
    public class PaymentProcessedEventHandler : IEventHandler<PaymentProcessedEvent>
    {
        private readonly ILogger<PaymentProcessedEventHandler> _logger;

        public PaymentProcessedEventHandler(ILogger<PaymentProcessedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> HandleAsync(PaymentProcessedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("PaymentProcessedEventHandler: Processing payment for order {OrderId}", domainEvent.OrderId);

                // 核心业务逻辑：支付处理确认
                await ProcessPaymentAsync(domainEvent, cancellationToken);

                _logger.LogInformation("PaymentProcessedEventHandler: Successfully processed payment for order {OrderId}", domainEvent.OrderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentProcessedEventHandler: Error processing payment for order {OrderId}", domainEvent.OrderId);
                return false;
            }
        }

        /// <summary>
        /// 处理支付的核心业务逻辑
        /// </summary>
        private async Task ProcessPaymentAsync(PaymentProcessedEvent domainEvent, CancellationToken cancellationToken)
        {
            // 1. 记录支付处理日志
            _logger.LogInformation("Payment {PaymentId} processed for order {OrderId} with amount {Amount} at {Timestamp}", 
                domainEvent.PaymentId, domainEvent.OrderId, domainEvent.Amount, domainEvent.OccurredOn);

            // 2. 根据支付状态执行相应的业务逻辑
            if (domainEvent.Success)
            {
                _logger.LogInformation("Payment {PaymentId} successful for order {OrderId}", domainEvent.PaymentId, domainEvent.OrderId);
            }
            else
            {
                _logger.LogWarning("Payment {PaymentId} failed for order {OrderId} with reason: {ErrorMessage}", 
                    domainEvent.PaymentId, domainEvent.OrderId, domainEvent.ErrorMessage);
            }

            // 3. 可以在这里添加其他核心业务逻辑
            // 比如：更新订单状态、发送通知等
            
            // 4. 模拟异步处理
            await Task.Delay(50, cancellationToken);
            
            _logger.LogInformation("Payment processing completed for payment {PaymentId}", domainEvent.PaymentId);
        }
    }
}
