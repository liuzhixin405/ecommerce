using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 支付处理事件处理器
    /// </summary>
    public class PaymentProcessedEventHandler : IEventHandler<PaymentProcessedEvent>
    {
        private readonly ILogger<PaymentProcessedEventHandler> _logger;
        private readonly INotificationService _notificationService;
        private readonly ICacheService _cacheService;

        public PaymentProcessedEventHandler(
            ILogger<PaymentProcessedEventHandler> logger,
            INotificationService notificationService,
            ICacheService cacheService)
        {
            _logger = logger;
            _notificationService = notificationService;
            _cacheService = cacheService;
        }

        public async Task<bool> HandleAsync(PaymentProcessedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("PaymentProcessedEventHandler: Processing payment for payment {PaymentId}", domainEvent.PaymentId);

                // 1. 发送支付通知
                await _notificationService.SendPaymentNotificationAsync(domainEvent);

                // 2. 清除相关缓存
                await _cacheService.RemoveByPatternAsync($"payment_{domainEvent.PaymentId}");
                await _cacheService.RemoveByPatternAsync("payments_list");

                // 3. 更新财务记录
                await UpdateFinancialRecordsAsync(domainEvent, cancellationToken);

                // 4. 发送系统通知
                await _notificationService.SendSystemAlertAsync(
                    $"Payment processed: {domainEvent.PaymentId} for amount {domainEvent.Amount}",
                    "Info");

                // 5. 记录支付日志
                await LogPaymentProcessedAsync(domainEvent, cancellationToken);

                _logger.LogInformation("PaymentProcessedEventHandler: Successfully processed payment {PaymentId}", domainEvent.PaymentId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentProcessedEventHandler: Error processing payment {PaymentId}", domainEvent.PaymentId);
                return false;
            }
        }

        private async Task UpdateFinancialRecordsAsync(PaymentProcessedEvent domainEvent, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("PaymentProcessedEventHandler: Updating financial records for payment {PaymentId}", domainEvent.PaymentId);
                
                // 在实际环境中，这里会调用财务系统API、更新会计记录等
                await Task.Delay(150, cancellationToken);
                
                _logger.LogInformation("PaymentProcessedEventHandler: Financial records updated successfully for payment {PaymentId}", domainEvent.PaymentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentProcessedEventHandler: Failed to update financial records for payment {PaymentId}", domainEvent.PaymentId);
            }
        }

        private async Task LogPaymentProcessedAsync(PaymentProcessedEvent domainEvent, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("PaymentProcessedEventHandler: Logging payment for payment {PaymentId} with amount {Amount}", 
                    domainEvent.PaymentId, domainEvent.Amount);
                
                // 在实际环境中，这里会记录到支付日志表
                await Task.Delay(50, cancellationToken);
                
                _logger.LogInformation("PaymentProcessedEventHandler: Payment logged successfully for payment {PaymentId}", domainEvent.PaymentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentProcessedEventHandler: Failed to log payment for payment {PaymentId}", domainEvent.PaymentId);
            }
        }
    }
}
