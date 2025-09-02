# 电商系统业务流程说明

## 核心业务流程

### 1. 用户注册流程
```
用户注册 → 创建用户 → 发布UserRegisteredEvent → 发送欢迎邮件 → 更新统计 → 完成
```

### 2. 下单流程
```
用户下单 → 检查库存 → 锁定库存 → 创建订单 → 发布OrderCreatedEvent → 发送确认邮件 → 完成
```

### 3. 支付流程
```
用户支付 → 处理支付 → 更新订单状态 → 发布OrderPaidEvent → 发送支付确认 → 更新库存 → 完成
```

### 4. 订单状态变更流程
```
状态变更 → 发布OrderStatusChangedEvent → 发送状态通知 → 更新相关数据 → 完成
```

### 5. 库存管理流程
```
库存变更 → 发布InventoryUpdatedEvent → 检查低库存 → 发送警告 → 更新缓存 → 完成
```

## 事件驱动架构

### 事件发布者
- **OrderService**: 发布订单相关事件
- **UserService**: 发布用户相关事件
- **InventoryService**: 发布库存相关事件
- **PaymentService**: 发布支付相关事件

### 事件处理器
- **OrderCreatedEventHandler**: 处理订单创建
- **OrderPaidEventHandler**: 处理订单支付
- **OrderStatusChangedEventHandler**: 处理订单状态变更
- **InventoryUpdatedEventHandler**: 处理库存更新
- **UserRegisteredEventHandler**: 处理用户注册

## 数据流向

```
用户操作 → Controller → Service → Repository → Database
                ↓
            发布事件 → EventBus → EventHandler → 异步处理
```

## 核心原则

1. **单一职责**: 每个服务只负责自己的业务领域
2. **事件驱动**: 通过事件实现服务间的松耦合
3. **异步处理**: 非关键操作异步处理，提高响应速度
4. **默认实现**: 外部服务（邮件、通知等）提供默认实现，便于开发和测试
