using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 订单状态变更事件处理器
    /// </summary>
    public class OrderStatusChangedEventHandler : IEventHandler<OrderPaidEvent>
    {
        private readonly ILogger<OrderStatusChangedEventHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IStatisticsService _statisticsService;
        private readonly ICacheService _cacheService;
        private readonly INotificationService _notificationService;

        public OrderStatusChangedEventHandler(
            ILogger<OrderStatusChangedEventHandler> logger,
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

        public async Task<bool> HandleAsync(OrderPaidEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("OrderStatusChangedEventHandler: Processing order status change to Paid for order {OrderId}", domainEvent.OrderId);

                // 1. 发送支付确认邮件
                var emailResult = await _emailService.SendPaymentConfirmationAsync(domainEvent);
                if (!emailResult)
                {
                    _logger.LogWarning("OrderStatusChangedEventHandler: Failed to send payment confirmation email for order {OrderId}", domainEvent.OrderId);
                }

                // 2. 更新支付统计信息
                await _statisticsService.UpdatePaymentStatisticsAsync(domainEvent);

                // 3. 更新销售统计信息
                await _statisticsService.UpdateSalesStatisticsAsync(domainEvent);

                // 4. 清除相关缓存
                await _cacheService.RemoveByPatternAsync($"order_{domainEvent.OrderId}");
                await _cacheService.RemoveByPatternAsync("orders_list");
                await _cacheService.RemoveByPatternAsync("sales_stats");

                // 5. 发送订单状态通知
                await _notificationService.SendOrderStatusNotificationAsync(
                    domainEvent.OrderId.ToString(),
                    "customer",
                    "Paid");

                // 6. 触发发货流程
                await TriggerShippingProcessAsync(domainEvent.OrderId, cancellationToken);

                // 7. 记录状态变更日志
                await LogOrderStatusChangeAsync(domainEvent, cancellationToken);

                _logger.LogInformation("OrderStatusChangedEventHandler: Successfully processed order status change for order {OrderId}", domainEvent.OrderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderStatusChangedEventHandler: Error processing order status change for order {OrderId}", domainEvent.OrderId);
                return false;
            }
        }

        private async Task TriggerShippingProcessAsync(Guid orderId, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("OrderStatusChangedEventHandler: Triggering shipping process for order {OrderId}", orderId);
                
                // 在实际环境中，这里会调用物流系统API、生成运单等
                await Task.Delay(200, cancellationToken);
                
                _logger.LogInformation("OrderStatusChangedEventHandler: Shipping process triggered successfully for order {OrderId}", orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderStatusChangedEventHandler: Failed to trigger shipping process for order {OrderId}", orderId);
            }
        }

        private async Task LogOrderStatusChangeAsync(OrderPaidEvent domainEvent, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("OrderStatusChangedEventHandler: Logging order status change for order {OrderId} to status Paid", domainEvent.OrderId);
                
                // 在实际环境中，这里会记录到订单状态变更日志表
                await Task.Delay(50, cancellationToken);
                
                _logger.LogInformation("OrderStatusChangedEventHandler: Order status change logged successfully for order {OrderId}", domainEvent.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderStatusChangedEventHandler: Failed to log order status change for order {OrderId}", domainEvent.OrderId);
            }
        }
    }
}
