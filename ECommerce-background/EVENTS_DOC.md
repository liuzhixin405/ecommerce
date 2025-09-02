## 事件发布 / 订阅文档（Event-driven Flow）

本文件整理系统内所有领域事件的 Publisher 与 Subscriber，以及关键行为与源码位置，便于业务串联与排查。

### 目录
- OrderCreatedEvent
- StockLockedEvent
- PaymentProcessedEvent
- PaymentSucceededEvent（新增）
- PaymentFailedEvent（新增）
- OrderPaidEvent
- InventoryUpdatedEvent
- OrderCancelledEvent
- OrderStatusChangedEvent
- 事件总线与订阅注册（RabbitMQEventBus + EventBusStartupService）

---

### OrderCreatedEvent
- 发布（Publisher）
  - 文件：`ECommerce.Application/Services/OrderService.cs`
  - 方法：`CreateOrderAsync`
  - 代码：
    ```
    var orderCreatedEvent = new OrderCreatedEvent(createdOrder);
    await _eventBus.PublishAsync(orderCreatedEvent);
    ```
- 订阅（Subscriber）
  - 文件：`ECommerce.Application/EventHandlers/OrderCreatedEventHandler.cs`
  - 行为：清空购物车、更新统计、发送订单创建通知、清理订单相关缓存

### StockLockedEvent
- 发布（Publisher）
  - 文件：`ECommerce.Application/Services/OrderService.cs`
  - 方法：`CreateOrderAsync`（为每个订单商品发布）
  - 代码：
    ```
    var stockLockedEvent = new StockLockedEvent(item.ProductId, item.Product?.Name ?? "Unknown Product", item.Quantity, createdOrder.Id);
    await _eventBus.PublishAsync(stockLockedEvent);
    ```
- 订阅（Subscriber）
  - 文件：`ECommerce.Application/EventHandlers/StockLockedEventHandler.cs`
  - 行为：记录锁定日志；发送锁定通知；可作为异步补偿/监控切入点

### PaymentProcessedEvent
- 发布（Publisher）
  - 文件：`ECommerce.Application/Services/OrderService.cs`
  - 方法：`ProcessPaymentAsync`
  - 代码：
    ```
    var paymentProcessedEvent = new PaymentProcessedEvent(paymentResult.PaymentId, order.Id, order.UserId, paymentDto.Amount, paymentDto.PaymentMethod, true);
    await _eventBus.PublishAsync(paymentProcessedEvent);
    ```
- 订阅（Subscriber）
  - 文件：`ECommerce.Application/EventHandlers/PaymentProcessedEventHandler.cs`
  - 行为：发送支付处理通知；成功时更新支付统计；失败时发送系统告警；清理订单缓存（注意：已拆分成功/失败事件以简化分支）

### PaymentSucceededEvent（新增）
- 发布（Publisher）
  - 文件：`ECommerce.Application/Services/OrderService.cs`
  - 方法：`ProcessPaymentAsync`（支付成功时）
  - 代码：
    ```
    var paymentSucceededEvent = new PaymentSucceededEvent(paymentResult.PaymentId, order.Id, order.UserId, paymentDto.Amount, paymentDto.PaymentMethod);
    await _eventBus.PublishAsync(paymentSucceededEvent);
    ```
- 订阅（Subscriber）
  - 文件：`ECommerce.Application/EventHandlers/PaymentSucceededEventHandler.cs`
  - 行为：更新支付/销售统计；发送支付成功通知；清理订单缓存

### PaymentFailedEvent（新增）
- 发布（Publisher）
  - 文件：`ECommerce.Application/Services/OrderService.cs`
  - 方法：`ProcessPaymentAsync`（支付失败时）
  - 代码：
    ```
    var paymentFailedEvent = new PaymentFailedEvent(paymentResult.PaymentId \?\? string.Empty, order.Id, order.UserId, paymentDto.Amount, paymentDto.PaymentMethod, paymentResult.Message);
    await _eventBus.PublishAsync(paymentFailedEvent);
    ```
- 订阅（Subscriber）
  - 文件：`ECommerce.Application/EventHandlers/PaymentFailedEventHandler.cs`
  - 行为：发送失败通知与系统告警；清理订单缓存

### OrderPaidEvent
- 发布（Publisher）
  - 文件：`ECommerce.Application/Services/OrderService.cs`
  - 方法：`ProcessPaymentAsync`
  - 代码：
    ```
    var orderPaidEvent = new OrderPaidEvent(order.Id, order.UserId, paymentResult.PaymentId, paymentDto.Amount, paymentDto.PaymentMethod);
    await _eventBus.PublishAsync(orderPaidEvent);
    ```
- 订阅（Subscriber）
  - 文件：`ECommerce.Application/EventHandlers/OrderPaidEventHandler.cs`
  - 行为：更新支付/销售统计；发送支付成功通知；清理订单缓存

- ### InventoryUpdatedEvent
- 发布（Publisher）
  - 文件：`ECommerce.Application/Services/InventoryService.cs`
  - 方法：
    - `DeductStockAsync`（扣减后发布）
    - `RestoreStockAsync`（已实现：恢复后发布，OperationType.Add）
    - `LockStockAsync`（已实现：锁定后发布，OperationType.Lock，库存数不变）
    - `ReleaseLockedStockAsync`（已实现：解锁后发布，OperationType.Unlock，库存数不变）
  - 代码示例（扣减）：
    ```
    var inventoryUpdatedEvent = new InventoryUpdatedEvent(productId, product.Name, oldStock, product.Stock, InventoryOperationType.Deduct, "Stock deduction", Guid.Empty);
    await _eventBus.PublishAsync(inventoryUpdatedEvent);
    ```
- 订阅（Subscriber）
  - 文件：`ECommerce.Application/EventHandlers/InventoryUpdatedEventHandler.cs`
  - 行为：同步产品库存到数据库；更新库存统计；发送库存变更/低库存通知；清理产品相关缓存

### OrderCancelledEvent
- 发布（Publisher）
  - 文件：`ECommerce.Application/Services/OrderService.cs`
  - 方法：`CancelOrderAsync`
  - 代码：
    ```
    var orderCancelledEvent = new OrderCancelledEvent(order.Id, order.UserId, "Order cancelled by user or system", order.Status);
    await _eventBus.PublishAsync(orderCancelledEvent);
    ```
- 订阅（Subscriber）
  - 文件：`ECommerce.Application/EventHandlers/OrderCancelledEventHandler.cs`
  - 行为：更新取消统计；发送取消通知；清理订单缓存

- ### OrderStatusChangedEvent
- 发布（Publisher）
  - 文件：`ECommerce.Application/Services/OrderService.cs`
  - 方法：`UpdateOrderStatusAsync`（已实现：更新成功后发布）
  - 代码：
    ```
    var statusChangedEvent = new OrderStatusChangedEvent(order.Id, order.UserId, oldStatus, order.Status);
    await _eventBus.PublishAsync(statusChangedEvent);
    ```
- 订阅（Subscriber）
  - 文件：`ECommerce.Application/EventHandlers/OrderStatusChangedEventHandler.cs`
  - 行为：记录状态变更；发送状态通知；清理订单缓存

---

## 事件总线与订阅清单

### 事件总线（RabbitMQEventBus）
- 文件：`ECommerce.Infrastructure/EventBus/RabbitMQEventBus.cs`
- 方法：`PublishAsync<T>` 将事件序列化为 JSON，发到交换机（BrokerName），路由键为事件名。
- 消费：`StartConsuming()` 在 `EventBusStartupService` 中调用，拉取队列消息并分发到对应 Handler。

### 订阅注册（EventBusStartupService）
- 文件：`ECommerce.Infrastructure/EventBus/EventBusStartupService.cs`
- 逻辑：服务启动时执行 `ConfigureEventSubscriptionsAsync()`，通过反射扫描 DI 容器已注册的所有 `IEventHandler<T>`，并调用 `EventSubscriptionManager.AddSubscription<T, TH>()` 统一按事件类型注册到 `RabbitMQEventBus`。
- 因此，只要 Handler 被注册进 DI（Program.cs 已 AddScoped），就会自动订阅。

### Program.cs 中的相关注册
- 文件：`ECommerce.API/Program.cs`
  - 注册事件总线：`builder.Services.AddRabbitMQEventBus(builder.Configuration);`
  - 注册事件处理器：`OrderCreatedEventHandler`、`OrderPaidEventHandler`、`OrderStatusChangedEventHandler`、`OrderCancelledEventHandler`、`InventoryUpdatedEventHandler`、`PaymentProcessedEventHandler`、`PaymentSucceededEventHandler`、`PaymentFailedEventHandler`、`StockLockedEventHandler`、`UserRegisteredEventHandler`
  - 启动服务：`builder.Services.AddHostedService<EventBusStartupService>();`

---

## 待补充发布点（建议）
- 在 `OrderService.UpdateOrderStatusAsync` 成功更新后统一发布 `OrderStatusChangedEvent`，保证状态流转都有事件记录与订阅端联动。
- 在 `InventoryService` 的 `RestoreStockAsync`、`LockStockAsync`、`ReleaseLockedStockAsync` 成功后发布 `InventoryUpdatedEvent`（OperationType 对应 `Add/Lock/Unlock`），完善库存事件闭环。


