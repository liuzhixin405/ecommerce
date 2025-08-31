using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 订单取消事件处理器
    /// </summary>
    public class OrderCancelledEventHandler : IEventHandler<OrderCancelledEvent>
    {
        private readonly ILogger<OrderCancelledEventHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IStatisticsService _statisticsService;
        private readonly ICacheService _cacheService;
        private readonly INotificationService _notificationService;

        public OrderCancelledEventHandler(
            ILogger<OrderCancelledEventHandler> logger,
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

        public async Task<bool> HandleAsync(OrderCancelledEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("OrderCancelledEventHandler: Processing order cancellation for order {OrderId}", domainEvent.OrderId);

                // 1. 发送订单取消确认邮件
                var emailResult = await _emailService.SendOrderCancellationAsync(domainEvent);
                if (!emailResult)
                {
                    _logger.LogWarning("OrderCancelledEventHandler: Failed to send order cancellation email for order {OrderId}", domainEvent.OrderId);
                }

                // 2. 更新取消统计信息
                await _statisticsService.UpdateCancellationStatisticsAsync(domainEvent);

                // 3. 清除相关缓存
                await _cacheService.RemoveByPatternAsync($"order_{domainEvent.OrderId}");
                await _cacheService.RemoveByPatternAsync("orders_list");

                // 4. 发送订单状态通知
                await _notificationService.SendOrderStatusNotificationAsync(
                    domainEvent.OrderId.ToString(),
                    "customer",
                    "Cancelled");

                // 5. 处理退款
                await ProcessRefundAsync(domainEvent.OrderId, domainEvent.TotalAmount, cancellationToken);

                // 6. 记录取消日志
                await LogOrderCancellationAsync(domainEvent, cancellationToken);

                _logger.LogInformation("OrderCancelledEventHandler: Successfully processed order cancellation for order {OrderId}", domainEvent.OrderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderCancelledEventHandler: Error processing order cancellation for order {OrderId}", domainEvent.OrderId);
                return false;
            }
        }

        private async Task ProcessRefundAsync(Guid orderId, decimal amount, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("OrderCancelledEventHandler: Processing refund for order {OrderId} with amount {Amount}", orderId, amount);
                
                // 在实际环境中，这里会调用支付网关的退款API、更新财务记录等
                await Task.Delay(300, cancellationToken);
                
                _logger.LogInformation("OrderCancelledEventHandler: Refund processed successfully for order {OrderId}", orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderCancelledEventHandler: Failed to process refund for order {OrderId}", orderId);
            }
        }

        private async Task LogOrderCancellationAsync(OrderCancelledEvent domainEvent, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("OrderCancelledEventHandler: Logging order cancellation for order {OrderId} with reason {Reason}", 
                    domainEvent.OrderId, domainEvent.Reason);
                
                // 在实际环境中，这里会记录到订单取消日志表
                await Task.Delay(50, cancellationToken);
                
                _logger.LogInformation("OrderCancelledEventHandler: Order cancellation logged successfully for order {OrderId}", domainEvent.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderCancelledEventHandler: Failed to log order cancellation for order {OrderId}", domainEvent.OrderId);
            }
        }
    }
}
