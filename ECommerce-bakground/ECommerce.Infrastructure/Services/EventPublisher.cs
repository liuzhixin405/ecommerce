using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ECommerce.Infrastructure.Services
{
    public class EventPublisher : IEventPublisher
    {
        private readonly ILogger<EventPublisher> _logger;
        private readonly IOutboxService _outboxService;
        private readonly IEventHandlerFactory _eventHandlerFactory;

        public EventPublisher(
            ILogger<EventPublisher> logger, 
            IOutboxService outboxService,
            IEventHandlerFactory eventHandlerFactory)
        {
            _logger = logger;
            _outboxService = outboxService;
            _eventHandlerFactory = eventHandlerFactory;
        }

        public async Task PublishAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                // 将事件添加到Outbox，确保事务一致性
                await _outboxService.AddEventAsync(domainEvent);
                
                _logger.LogInformation("Event {EventType} with ID {EventId} added to outbox for publishing", 
                    domainEvent.Type, domainEvent.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish event {EventType} with ID {EventId}", 
                    domainEvent.Type, domainEvent.Id);
                throw;
            }
        }

        public async Task PublishAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            try
            {
                // 批量添加事件到Outbox
                await _outboxService.AddEventsAsync(domainEvents);
                
                _logger.LogInformation("Added {Count} events to outbox for publishing", domainEvents.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish {Count} events", domainEvents.Count());
                throw;
            }
        }

        public async Task PublishOutboxMessageAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken = default)
        {
            try
            {
                // 标记为处理中
                await _outboxService.MarkAsProcessingAsync(outboxMessage.Id);

                // 这里可以实现实际的事件发布逻辑
                // 例如：发布到消息队列、调用外部服务等
                await PublishToExternalSystemsAsync(outboxMessage, cancellationToken);

                // 标记为已完成
                await _outboxService.MarkAsCompletedAsync(outboxMessage.Id);

                _logger.LogInformation("Successfully published outbox message {MessageId} of type {MessageType}", 
                    outboxMessage.Id, outboxMessage.Type);
            }
            catch (Exception ex)
            {
                // 标记为失败，设置重试时间
                var retryAfter = DateTime.UtcNow.AddMinutes(5); // 5分钟后重试
                await _outboxService.MarkAsFailedAsync(outboxMessage.Id, ex.Message, retryAfter);

                _logger.LogError(ex, "Failed to publish outbox message {MessageId} of type {MessageType}", 
                    outboxMessage.Id, outboxMessage.Type);
                throw;
            }
        }

        private async Task PublishToExternalSystemsAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Publishing message {MessageId} of type {EventType} to external systems", 
                    outboxMessage.Id, outboxMessage.Type);

                // 使用EventHandlerFactory获取并执行相应的处理器
                var handlers = _eventHandlerFactory.GetHandlers(outboxMessage.Type);
                
                if (handlers.Any())
                {
                    foreach (var handler in handlers)
                    {
                        try
                        {
                            // 通过反射调用HandleAsync方法
                            var handleMethod = handler.GetType().GetMethod("HandleAsync");
                            if (handleMethod != null)
                            {
                                // 反序列化事件数据
                                var eventType = Type.GetType($"ECommerce.Domain.Models.{outboxMessage.Type}");
                                if (eventType != null)
                                {
                                    var domainEvent = System.Text.Json.JsonSerializer.Deserialize(outboxMessage.Data, eventType);
                                    if (domainEvent != null)
                                    {
                                        var result = await (Task<bool>)handleMethod.Invoke(handler, new[] { domainEvent, cancellationToken });
                                        _logger.LogInformation("Handler {HandlerType} processed event {EventType} with result: {Result}", 
                                            handler.GetType().Name, outboxMessage.Type, result);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error executing handler {HandlerType} for event {EventType}", 
                                handler.GetType().Name, outboxMessage.Type);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No handlers found for event type: {EventType}", outboxMessage.Type);
                }

                // 模拟异步处理
                await Task.Delay(100, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message {MessageId} to external systems", outboxMessage.Id);
                throw;
            }
        }

        private async Task HandleOrderCreatedEventAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
        {
            var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(outboxMessage.Data);
            if (orderEvent != null)
            {
                _logger.LogInformation("Processing OrderCreatedEvent for order {OrderId}", orderEvent.OrderId);
                
                try
                {
                    // 1. 发送订单确认邮件
                    await _emailService.SendOrderConfirmationAsync(orderEvent);
                    
                    // 2. 更新订单统计信息
                    await _statisticsService.UpdateOrderStatisticsAsync(orderEvent);
                    
                    // 3. 清除相关缓存
                    await _cacheService.RemoveByPatternAsync($"order_{orderEvent.OrderId}");
                    await _cacheService.RemoveByPatternAsync("orders_list");
                    
                    // 4. 发送系统通知
                    await _notificationService.SendOrderStatusNotificationAsync(
                        orderEvent.OrderId.ToString(), 
                        "customer", 
                        "Created");
                    
                    _logger.LogInformation("OrderCreatedEvent processed successfully for order {OrderId}", orderEvent.OrderId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing OrderCreatedEvent for order {OrderId}", orderEvent.OrderId);
                    throw;
                }
            }
        }

        private async Task HandleOrderPaidEventAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
        {
            var orderEvent = JsonSerializer.Deserialize<OrderPaidEvent>(outboxMessage.Data);
            if (orderEvent != null)
            {
                _logger.LogInformation("Processing OrderPaidEvent for order {OrderId}", orderEvent.OrderId);
                
                try
                {
                    // 1. 发送支付确认邮件
                    await _emailService.SendPaymentConfirmationAsync(orderEvent);
                    
                    // 2. 更新支付统计信息
                    await _statisticsService.UpdatePaymentStatisticsAsync(orderEvent);
                    
                    // 3. 更新销售统计信息
                    await _statisticsService.UpdateSalesStatisticsAsync(orderEvent);
                    
                    // 4. 清除相关缓存
                    await _cacheService.RemoveByPatternAsync($"order_{orderEvent.OrderId}");
                    await _cacheService.RemoveByPatternAsync("orders_list");
                    await _cacheService.RemoveByPatternAsync("sales_stats");
                    
                    // 5. 发送订单状态通知
                    await _notificationService.SendOrderStatusNotificationAsync(
                        orderEvent.OrderId.ToString(), 
                        "customer", 
                        "Paid");
                    
                    // 6. 触发发货流程（这里可以添加实际的发货逻辑）
                    await TriggerShippingProcessAsync(orderEvent.OrderId, cancellationToken);
                    
                    _logger.LogInformation("OrderPaidEvent processed successfully for order {OrderId}", orderEvent.OrderId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing OrderPaidEvent for order {OrderId}", orderEvent.OrderId);
                    throw;
                }
            }
        }

        private async Task HandleOrderCancelledEventAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
        {
            var orderEvent = JsonSerializer.Deserialize<OrderCancelledEvent>(outboxMessage.Data);
            if (orderEvent != null)
            {
                _logger.LogInformation("Processing OrderCancelledEvent for order {OrderId}", orderEvent.OrderId);
                
                try
                {
                    // 1. 发送订单取消确认邮件
                    await _emailService.SendOrderCancellationAsync(orderEvent);
                    
                    // 2. 更新取消统计信息
                    await _statisticsService.UpdateCancellationStatisticsAsync(orderEvent);
                    
                    // 3. 清除相关缓存
                    await _cacheService.RemoveByPatternAsync($"order_{orderEvent.OrderId}");
                    await _cacheService.RemoveByPatternAsync("orders_list");
                    
                    // 4. 发送订单状态通知
                    await _notificationService.SendOrderStatusNotificationAsync(
                        orderEvent.OrderId.ToString(), 
                        "customer", 
                        "Cancelled");
                    
                    // 5. 处理退款（这里可以添加实际的退款逻辑）
                    await ProcessRefundAsync(orderEvent.OrderId, orderEvent.TotalAmount, cancellationToken);
                    
                    _logger.LogInformation("OrderCancelledEvent processed successfully for order {OrderId}", orderEvent.OrderId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing OrderCancelledEvent for order {OrderId}", orderEvent.OrderId);
                    throw;
                }
            }
        }

        private async Task HandleInventoryUpdatedEventAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
        {
            var inventoryEvent = JsonSerializer.Deserialize<InventoryUpdatedEvent>(outboxMessage.Data);
            if (inventoryEvent != null)
            {
                _logger.LogInformation("Processing InventoryUpdatedEvent for product {ProductId}", inventoryEvent.ProductId);
                
                try
                {
                    // 1. 更新库存统计信息
                    await _statisticsService.UpdateInventoryStatisticsAsync(inventoryEvent);
                    
                    // 2. 清除相关缓存
                    await _cacheService.RemoveByPatternAsync($"product_{inventoryEvent.ProductId}");
                    await _cacheService.RemoveByPatternAsync("products_list");
                    await _cacheService.RemoveByPatternAsync("inventory_stats");
                    
                    // 3. 检查是否需要发送低库存警告
                    if (inventoryEvent.NewStock <= 10) // 假设库存低于10时发送警告
                    {
                        await _emailService.SendLowStockAlertAsync(inventoryEvent);
                        await _notificationService.SendLowStockNotificationAsync(inventoryEvent);
                    }
                    
                    // 4. 更新产品缓存
                    await _cacheService.SetAsync($"product_{inventoryEvent.ProductId}_stock", inventoryEvent.NewStock, TimeSpan.FromMinutes(30));
                    
                    _logger.LogInformation("InventoryUpdatedEvent processed successfully for product {ProductId}", inventoryEvent.ProductId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing InventoryUpdatedEvent for product {ProductId}", inventoryEvent.ProductId);
                    throw;
                }
            }
        }

        private async Task HandlePaymentProcessedEventAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
        {
            var paymentEvent = JsonSerializer.Deserialize<PaymentProcessedEvent>(outboxMessage.Data);
            if (paymentEvent != null)
            {
                _logger.LogInformation("Processing PaymentProcessedEvent for payment {PaymentId}", paymentEvent.PaymentId);
                
                try
                {
                    // 1. 发送支付通知
                    await _notificationService.SendPaymentNotificationAsync(paymentEvent);
                    
                    // 2. 清除相关缓存
                    await _cacheService.RemoveByPatternAsync($"payment_{paymentEvent.PaymentId}");
                    await _cacheService.RemoveByPatternAsync("payments_list");
                    
                    // 3. 更新财务记录（这里可以添加实际的财务系统集成）
                    await UpdateFinancialRecordsAsync(paymentEvent, cancellationToken);
                    
                    // 4. 发送系统通知
                    await _notificationService.SendSystemAlertAsync(
                        $"Payment processed: {paymentEvent.PaymentId} for amount {paymentEvent.Amount}", 
                        "Info");
                    
                    _logger.LogInformation("PaymentProcessedEvent processed successfully for payment {PaymentId}", paymentEvent.PaymentId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing PaymentProcessedEvent for payment {PaymentId}", paymentEvent.PaymentId);
                    throw;
                }
            }
        }

        private async Task HandleStockLockedEventAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
        {
            var stockEvent = JsonSerializer.Deserialize<StockLockedEvent>(outboxMessage.Data);
            if (stockEvent != null)
            {
                _logger.LogInformation("Processing StockLockedEvent for product {ProductId}", stockEvent.ProductId);
                
                try
                {
                    // 1. 发送库存锁定通知
                    await _notificationService.SendInventoryAlertAsync(stockEvent);
                    
                    // 2. 清除相关缓存
                    await _cacheService.RemoveByPatternAsync($"product_{stockEvent.ProductId}");
                    await _cacheService.RemoveByPatternAsync("products_list");
                    
                    // 3. 更新库存缓存
                    await _cacheService.SetAsync($"product_{stockEvent.ProductId}_locked", stockEvent.Quantity, TimeSpan.FromMinutes(30));
                    
                    // 4. 检查是否需要发送低库存警告
                    await CheckAndSendLowStockWarningAsync(stockEvent.ProductId, cancellationToken);
                    
                    _logger.LogInformation("StockLockedEvent processed successfully for product {ProductId}", stockEvent.ProductId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing StockLockedEvent for product {ProductId}", stockEvent.ProductId);
                    throw;
                }
            }
        }

        private async Task HandleStockReleasedEventAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
        {
            var stockEvent = JsonSerializer.Deserialize<StockReleasedEvent>(outboxMessage.Data);
            if (stockEvent != null)
            {
                _logger.LogInformation("Processing StockReleasedEvent for product {ProductId}", stockEvent.ProductId);
                
                try
                {
                    // 1. 清除相关缓存
                    await _cacheService.RemoveByPatternAsync($"product_{stockEvent.ProductId}");
                    await _cacheService.RemoveByPatternAsync("products_list");
                    
                    // 2. 更新库存缓存
                    await _cacheService.SetAsync($"product_{stockEvent.ProductId}_released", stockEvent.Quantity, TimeSpan.FromMinutes(30));
                    
                    // 3. 发送库存恢复通知
                    await _notificationService.SendCustomerNotificationAsync(
                        "system", 
                        $"Stock released for product {stockEvent.ProductId}: {stockEvent.Quantity} units", 
                        "Inventory");
                    
                    // 4. 更新库存统计
                    await _statisticsService.UpdateInventoryStatisticsAsync(new InventoryUpdatedEvent
                    {
                        ProductId = stockEvent.ProductId,
                        ProductName = stockEvent.ProductName,
                        OldStock = 0, // 假设从0开始
                        NewStock = stockEvent.Quantity,
                        OperationType = "StockReleased",
                        UpdatedAt = DateTime.UtcNow
                    });
                    
                    _logger.LogInformation("StockReleasedEvent processed successfully for product {ProductId}", stockEvent.ProductId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing StockReleasedEvent for product {ProductId}", stockEvent.ProductId);
                    throw;
                }
            }
        }

        // 辅助方法：触发发货流程
        private async Task TriggerShippingProcessAsync(Guid orderId, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Triggering shipping process for order {OrderId}", orderId);
                
                // 默认实现：记录发货流程触发日志
                // 在实际环境中，这里会调用物流系统API、生成运单等
                await Task.Delay(200); // 模拟发货流程处理延迟
                
                _logger.LogInformation("Shipping process triggered successfully for order {OrderId}", orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to trigger shipping process for order {OrderId}", orderId);
                // 发货流程失败不应该影响订单支付事件的处理
            }
        }

        // 辅助方法：处理退款
        private async Task ProcessRefundAsync(Guid orderId, decimal amount, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processing refund for order {OrderId} with amount {Amount}", orderId, amount);
                
                // 默认实现：记录退款处理日志
                // 在实际环境中，这里会调用支付网关的退款API、更新财务记录等
                await Task.Delay(300); // 模拟退款处理延迟
                
                _logger.LogInformation("Refund processed successfully for order {OrderId}", orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process refund for order {OrderId}", orderId);
                // 退款处理失败不应该影响订单取消事件的处理
            }
        }

        // 辅助方法：更新财务记录
        private async Task UpdateFinancialRecordsAsync(PaymentProcessedEvent paymentEvent, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating financial records for payment {PaymentId}", paymentEvent.PaymentId);
                
                // 默认实现：记录财务记录更新日志
                // 在实际环境中，这里会调用财务系统API、更新会计记录等
                await Task.Delay(150); // 模拟财务记录更新延迟
                
                _logger.LogInformation("Financial records updated successfully for payment {PaymentId}", paymentEvent.PaymentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update financial records for payment {PaymentId}", paymentEvent.PaymentId);
                // 财务记录更新失败不应该影响支付事件的处理
            }
        }

        // 辅助方法：检查并发送低库存警告
        private async Task CheckAndSendLowStockWarningAsync(int productId, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Checking low stock warning for product {ProductId}", productId);
                
                // 默认实现：记录低库存检查日志
                // 在实际环境中，这里会查询数据库获取当前库存、判断是否需要发送警告等
                await Task.Delay(100); // 模拟低库存检查延迟
                
                _logger.LogInformation("Low stock warning check completed for product {ProductId}", productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check low stock warning for product {ProductId}", productId);
                // 低库存检查失败不应该影响库存锁定事件的处理
            }
        }

        // 新增事件类型的处理器方法
        private async Task HandleUserRegisteredEventAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
        {
            var userEvent = JsonSerializer.Deserialize<UserRegisteredEvent>(outboxMessage.Data);
            if (userEvent != null)
            {
                _logger.LogInformation("Processing UserRegisteredEvent for user {UserId}", userEvent.UserId);
                
                try
                {
                    // 1. 发送欢迎邮件
                    await _emailService.SendWelcomeEmailAsync(userEvent.Email, userEvent.UserName);
                    
                    // 2. 更新用户活动统计
                    await _statisticsService.UpdateUserActivityStatisticsAsync(userEvent.UserId, "Registered");
                    
                    // 3. 清除相关缓存
                    await _cacheService.RemoveByPatternAsync("users_list");
                    
                    // 4. 发送系统通知
                    await _notificationService.SendSystemAlertAsync(
                        $"New user registered: {userEvent.UserName} ({userEvent.Email})", 
                        "Info");
                    
                    _logger.LogInformation("UserRegisteredEvent processed successfully for user {UserId}", userEvent.UserId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing UserRegisteredEvent for user {UserId}", userEvent.UserId);
                    throw;
                }
            }
        }

        private async Task HandleUserLoggedInEventAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
        {
            var userEvent = JsonSerializer.Deserialize<UserLoggedInEvent>(outboxMessage.Data);
            if (userEvent != null)
            {
                _logger.LogInformation("Processing UserLoggedInEvent for user {UserId}", userEvent.UserId);
                
                try
                {
                    // 1. 更新用户活动统计
                    await _statisticsService.UpdateUserActivityStatisticsAsync(userEvent.UserId, "LoggedIn");
                    
                    // 2. 记录登录信息（这里可以添加实际的登录日志记录）
                    await LogUserLoginAsync(userEvent, cancellationToken);
                    
                    _logger.LogInformation("UserLoggedInEvent processed successfully for user {UserId}", userEvent.UserId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing UserLoggedInEvent for user {UserId}", userEvent.UserId);
                    throw;
                }
            }
        }

        private async Task HandleProductViewedEventAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
        {
            var productEvent = JsonSerializer.Deserialize<ProductViewedEvent>(outboxMessage.Data);
            if (productEvent != null)
            {
                _logger.LogInformation("Processing ProductViewedEvent for product {ProductId}", productEvent.ProductId);
                
                try
                {
                    // 1. 更新产品浏览统计
                    await _statisticsService.UpdateProductViewStatisticsAsync(productEvent.ProductId);
                    
                    // 2. 更新用户活动统计
                    if (!string.IsNullOrEmpty(productEvent.UserId))
                    {
                        await _statisticsService.UpdateUserActivityStatisticsAsync(productEvent.UserId, "ProductViewed");
                    }
                    
                    // 3. 缓存产品浏览信息（用于推荐系统）
                    await _cacheService.SetAsync($"product_{productEvent.ProductId}_viewed_{productEvent.UserId}", 
                        DateTime.UtcNow, TimeSpan.FromHours(24));
                    
                    _logger.LogInformation("ProductViewedEvent processed successfully for product {ProductId}", productEvent.ProductId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing ProductViewedEvent for product {ProductId}", productEvent.ProductId);
                    throw;
                }
            }
        }

        private async Task HandleCartUpdatedEventAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
        {
            var cartEvent = JsonSerializer.Deserialize<CartUpdatedEvent>(outboxMessage.Data);
            if (cartEvent != null)
            {
                _logger.LogInformation("Processing CartUpdatedEvent for user {UserId} and product {ProductId}", 
                    cartEvent.UserId, cartEvent.ProductId);
                
                try
                {
                    // 1. 更新用户活动统计
                    await _statisticsService.UpdateUserActivityStatisticsAsync(cartEvent.UserId, "CartUpdated");
                    
                    // 2. 清除购物车相关缓存
                    await _cacheService.RemoveByPatternAsync($"cart_{cartEvent.UserId}");
                    
                    // 3. 记录购物车活动（用于分析用户行为）
                    await LogCartActivityAsync(cartEvent, cancellationToken);
                    
                    _logger.LogInformation("CartUpdatedEvent processed successfully for user {UserId}", cartEvent.UserId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing CartUpdatedEvent for user {UserId}", cartEvent.UserId);
                    throw;
                }
            }
        }

        private async Task HandleOrderShippedEventAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
        {
            var orderEvent = JsonSerializer.Deserialize<OrderShippedEvent>(outboxMessage.Data);
            if (orderEvent != null)
            {
                _logger.LogInformation("Processing OrderShippedEvent for order {OrderId}", orderEvent.OrderId);
                
                try
                {
                    // 1. 发送发货通知邮件
                    await _emailService.SendOrderShippedAsync(
                        orderEvent.OrderId.ToString(), 
                        orderEvent.CustomerEmail, 
                        orderEvent.TrackingNumber);
                    
                    // 2. 清除相关缓存
                    await _cacheService.RemoveByPatternAsync($"order_{orderEvent.OrderId}");
                    await _cacheService.RemoveByPatternAsync("orders_list");
                    
                    // 3. 发送订单状态通知
                    await _notificationService.SendOrderStatusNotificationAsync(
                        orderEvent.OrderId.ToString(), 
                        "customer", 
                        "Shipped");
                    
                    _logger.LogInformation("OrderShippedEvent processed successfully for order {OrderId}", orderEvent.OrderId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing OrderShippedEvent for order {OrderId}", orderEvent.OrderId);
                    throw;
                }
            }
        }

        private async Task HandleOrderDeliveredEventAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
        {
            var orderEvent = JsonSerializer.Deserialize<OrderDeliveredEvent>(outboxMessage.Data);
            if (orderEvent != null)
            {
                _logger.LogInformation("Processing OrderDeliveredEvent for order {OrderId}", orderEvent.OrderId);
                
                try
                {
                    // 1. 清除相关缓存
                    await _cacheService.RemoveByPatternAsync($"order_{orderEvent.OrderId}");
                    await _cacheService.RemoveByPatternAsync("orders_list");
                    
                    // 2. 发送订单状态通知
                    await _notificationService.SendOrderStatusNotificationAsync(
                        orderEvent.OrderId.ToString(), 
                        "customer", 
                        "Delivered");
                    
                    // 3. 更新订单完成统计
                    await _statisticsService.UpdateOrderStatisticsAsync(new OrderCreatedEvent
                    {
                        OrderId = orderEvent.OrderId,
                        OrderDate = orderEvent.DeliveredAt
                    });
                    
                    _logger.LogInformation("OrderDeliveredEvent processed successfully for order {OrderId}", orderEvent.OrderId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing OrderDeliveredEvent for order {OrderId}", orderEvent.OrderId);
                    throw;
                }
            }
        }

        private async Task HandleRefundProcessedEventAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
        {
            var refundEvent = JsonSerializer.Deserialize<RefundProcessedEvent>(outboxMessage.Data);
            if (refundEvent != null)
            {
                _logger.LogInformation("Processing RefundProcessedEvent for order {OrderId}", refundEvent.OrderId);
                
                try
                {
                    // 1. 发送退款确认邮件
                    await _emailService.SendRefundConfirmationAsync(
                        refundEvent.OrderId.ToString(), 
                        refundEvent.CustomerEmail, 
                        refundEvent.Amount);
                    
                    // 2. 清除相关缓存
                    await _cacheService.RemoveByPatternAsync($"order_{refundEvent.OrderId}");
                    await _cacheService.RemoveByPatternAsync("orders_list");
                    await _cacheService.RemoveByPatternAsync("refunds_list");
                    
                    // 3. 发送订单状态通知
                    await _notificationService.SendOrderStatusNotificationAsync(
                        refundEvent.OrderId.ToString(), 
                        "customer", 
                        "Refunded");
                    
                    _logger.LogInformation("RefundProcessedEvent processed successfully for order {OrderId}", refundEvent.OrderId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing RefundProcessedEvent for order {OrderId}", refundEvent.OrderId);
                    throw;
                }
            }
        }

        private async Task HandleLowStockAlertEventAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
        {
            var alertEvent = JsonSerializer.Deserialize<LowStockAlertEvent>(outboxMessage.Data);
            if (alertEvent != null)
            {
                _logger.LogInformation("Processing LowStockAlertEvent for product {ProductId}", alertEvent.ProductId);
                
                try
                {
                    // 1. 发送低库存警告邮件
                    await _emailService.SendLowStockAlertAsync(new InventoryUpdatedEvent
                    {
                        ProductId = alertEvent.ProductId,
                        ProductName = alertEvent.ProductName,
                        NewStock = alertEvent.CurrentStock,
                        OperationType = "LowStockAlert",
                        UpdatedAt = alertEvent.AlertedAt
                    });
                    
                    // 2. 发送系统通知
                    await _notificationService.SendSystemAlertAsync(
                        $"Low stock alert for product {alertEvent.ProductName} (ID: {alertEvent.ProductId}): Current stock {alertEvent.CurrentStock} is below threshold {alertEvent.Threshold}", 
                        "Warning");
                    
                    _logger.LogInformation("LowStockAlertEvent processed successfully for product {ProductId}", alertEvent.ProductId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing LowStockAlertEvent for product {ProductId}", alertEvent.ProductId);
                    throw;
                }
            }
        }

        // 辅助方法：记录用户登录
        private async Task LogUserLoginAsync(UserLoggedInEvent userEvent, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Logging user login for user {UserId} from IP {IpAddress}", 
                    userEvent.UserId, userEvent.IpAddress);
                
                // 默认实现：记录用户登录日志
                // 在实际环境中，这里会记录到数据库或日志系统
                await Task.Delay(50);
                
                _logger.LogInformation("User login logged successfully for user {UserId}", userEvent.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log user login for user {UserId}", userEvent.UserId);
            }
        }

        // 辅助方法：记录购物车活动
        private async Task LogCartActivityAsync(CartUpdatedEvent cartEvent, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Logging cart activity for user {UserId} with action {Action}", 
                    cartEvent.UserId, cartEvent.Action);
                
                // 默认实现：记录购物车活动日志
                // 在实际环境中，这里会记录到数据库或分析系统
                await Task.Delay(50);
                
                _logger.LogInformation("Cart activity logged successfully for user {UserId}", cartEvent.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log cart activity for user {UserId}", cartEvent.UserId);
            }
        }
    }
}
