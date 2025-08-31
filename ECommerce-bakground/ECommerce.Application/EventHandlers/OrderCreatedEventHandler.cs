using ECommerce.Core.EventBus;
using ECommerce.Domain.Events;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 订单创建事件处理器
    /// </summary>
    public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedEventHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IStatisticsService _statisticsService;
        private readonly ICacheService _cacheService;
        private readonly INotificationService _notificationService;

        public OrderCreatedEventHandler(
            ILogger<OrderCreatedEventHandler> logger,
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

        public async Task<bool> HandleAsync(OrderCreatedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("OrderCreatedEventHandler: Processing order creation for order {OrderId}", domainEvent.OrderId);

                // 1. 发送订单确认邮件
                var emailResult = await _emailService.SendOrderConfirmationAsync(domainEvent);
                if (!emailResult)
                {
                    _logger.LogWarning("OrderCreatedEventHandler: Failed to send order confirmation email for order {OrderId}", domainEvent.OrderId);
                }

                // 2. 更新订单统计信息
                await _statisticsService.UpdateOrderStatisticsAsync(domainEvent);

                // 3. 清除相关缓存
                await _cacheService.RemoveByPatternAsync($"order_{domainEvent.OrderId}");
                await _cacheService.RemoveByPatternAsync("orders_list");

                // 4. 发送系统通知
                await _notificationService.SendOrderStatusNotificationAsync(
                    domainEvent.OrderId.ToString(),
                    "customer",
                    "Created");

                // 5. 记录订单创建日志
                await LogOrderCreationAsync(domainEvent, cancellationToken);

                _logger.LogInformation("OrderCreatedEventHandler: Successfully processed order creation for order {OrderId}", domainEvent.OrderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderCreatedEventHandler: Error processing order creation for order {OrderId}", domainEvent.OrderId);
                return false;
            }
        }

        private async Task LogOrderCreationAsync(OrderCreatedEvent domainEvent, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("OrderCreatedEventHandler: Logging order creation for order {OrderId} with amount {Amount}", 
                    domainEvent.OrderId, domainEvent.TotalAmount);
                
                // 在实际环境中，这里会记录到专门的订单日志表或日志系统
                await Task.Delay(50, cancellationToken);
                
                _logger.LogInformation("OrderCreatedEventHandler: Order creation logged successfully for order {OrderId}", domainEvent.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderCreatedEventHandler: Failed to log order creation for order {OrderId}", domainEvent.OrderId);
            }
        }
    }
}