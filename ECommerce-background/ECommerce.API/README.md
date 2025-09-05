# ECommerce 电商系统后端文档

## 项目概述

这是一个基于 .NET 9 的现代化电商系统后端，采用事件驱动架构和微服务设计模式。

## 技术栈

- **框架**: .NET 9, ASP.NET Core Web API
- **数据库**: MySQL 8.0+ (Entity Framework Core)
- **消息队列**: RabbitMQ
- **认证**: JWT Bearer Token
- **架构模式**: Clean Architecture + Event-Driven Architecture

## 项目结构

```
ECommerce.API/                    # Web API 层
├── Controllers/                  # HTTP 控制器
│   ├── AuthController.cs        # 认证控制器
│   ├── ProductsController.cs    # 产品控制器
│   ├── OrdersController.cs      # 订单控制器
│   ├── UsersController.cs       # 用户控制器
│   ├── InventoryController.cs   # 库存控制器
│   ├── PaymentController.cs     # 支付控制器
│   └── InventoryTransactionsController.cs
├── BackgroundServices/          # 后台服务
│   └── OrderExpirationService.cs
├── Extensions/                  # 扩展方法
│   ├── GlobalExceptionMiddleware.cs
│   └── ServiceCollectionExtensions.cs
├── Program.cs                   # 应用程序入口
└── appsettings.json            # 配置文件

ECommerce.Application/           # 应用服务层
├── Services/                    # 业务服务
│   ├── AuthService.cs          # 认证服务
│   ├── UserService.cs          # 用户服务
│   ├── ProductService.cs       # 产品服务
│   ├── OrderService.cs         # 订单服务
│   ├── InventoryService.cs     # 库存服务
│   └── DefaultPaymentService.cs
├── EventHandlers/              # 事件处理器
│   ├── OrderCreatedEventHandler.cs
│   ├── OrderPaidEventHandler.cs
│   ├── OrderCancelledEventHandler.cs
│   ├── OrderStatusChangedEventHandler.cs
│   ├── InventoryUpdatedEventHandler.cs
│   ├── PaymentProcessedEventHandler.cs
│   ├── PaymentSucceededEventHandler.cs
│   ├── PaymentFailedEventHandler.cs
│   ├── StockLockedEventHandler.cs
│   └── UserRegisteredEventHandler.cs

ECommerce.Domain/                # 领域层
├── Entities/                   # 实体
│   ├── User.cs                # 用户实体
│   ├── Product.cs             # 产品实体
│   ├── Order.cs               # 订单实体
│   └── ShoppingCart.cs        # 购物车实体
├── Events/                    # 领域事件
│   ├── BaseEvent.cs           # 事件基类
│   ├── OrderCreatedEvent.cs   # 订单创建事件
│   ├── OrderPaidEvent.cs      # 订单支付事件
│   ├── OrderCancelledEvent.cs # 订单取消事件
│   ├── PaymentSucceededEvent.cs
│   ├── PaymentFailedEvent.cs
│   ├── StockLockedEvent.cs    # 库存锁定事件
│   └── InventoryUpdatedEvent.cs
├── Interfaces/                # 仓储接口
│   ├── IUserRepository.cs
│   ├── IProductRepository.cs
│   ├── IOrderRepository.cs
│   └── IInventoryTransactionRepository.cs
└── Models/                    # 数据模型
    ├── UserDto.cs
    ├── ProductDto.cs
    ├── OrderDto.cs
    └── JwtSettings.cs

ECommerce.Infrastructure/        # 基础设施层
├── Data/                      # 数据访问
│   └── ECommerceDbContext.cs  # 数据库上下文
├── EventBus/                  # 事件总线
│   ├── IEventBus.cs
│   ├── RabbitMQEventBus.cs
│   ├── InMemoryEventBus.cs
│   └── EventBusStartupService.cs
├── MessageQueue/              # 消息队列
│   └── RabbitMQService.cs
├── Repositories/              # 仓储实现
│   ├── UserRepository.cs
│   ├── ProductRepository.cs
│   ├── OrderRepository.cs
│   └── InventoryTransactionRepository.cs
└── Services/                  # 基础设施服务
    ├── DefaultEmailService.cs
    ├── DefaultCacheService.cs
    ├── DefaultNotificationService.cs
    ├── DefaultStatisticsService.cs
    └── JwtService.cs

ECommerce.Core/                 # 核心层
└── EventBus/
    └── IEventBus.cs           # 事件总线接口
```

## 核心功能

### 1. 用户管理
- 用户注册/登录
- JWT 认证
- 刷新令牌机制
- 用户信息管理

### 2. 产品管理
- 产品 CRUD 操作
- 产品分类管理
- 产品搜索和过滤
- 产品库存管理

### 3. 订单管理
- 订单创建
- 订单状态管理
- 订单支付处理
- 订单取消和退款
- 订单超时自动取消

### 4. 库存管理
- 库存检查
- 库存锁定/释放
- 库存扣减/恢复
- 库存事务记录

### 5. 购物车
- 购物车商品管理
- 购物车持久化
- 购物车清理

### 6. 支付处理
- 支付流程处理
- 支付成功/失败事件
- 支付状态跟踪

## 事件驱动架构

### 核心事件

1. **OrderCreatedEvent** - 订单创建
   - 发布者: OrderService.CreateOrderAsync
   - 订阅者: OrderCreatedEventHandler
   - 行为: 清空购物车、更新统计、发送通知

2. **StockLockedEvent** - 库存锁定
   - 发布者: OrderService.CreateOrderAsync
   - 订阅者: StockLockedEventHandler
   - 行为: 记录锁定操作、发送监控告警

3. **PaymentSucceededEvent** - 支付成功
   - 发布者: OrderService.ProcessPaymentAsync
   - 订阅者: PaymentSucceededEventHandler
   - 行为: 更新支付统计、发送成功通知

4. **PaymentFailedEvent** - 支付失败
   - 发布者: OrderService.ProcessPaymentAsync
   - 订阅者: PaymentFailedEventHandler
   - 行为: 发送失败通知、系统告警

5. **OrderPaidEvent** - 订单支付
   - 发布者: OrderService.ProcessPaymentAsync
   - 订阅者: OrderPaidEventHandler
   - 行为: 更新销售统计、发送支付通知

6. **InventoryUpdatedEvent** - 库存更新
   - 发布者: InventoryService (各种操作)
   - 订阅者: InventoryUpdatedEventHandler
   - 行为: 同步库存、更新统计、低库存告警

7. **OrderCancelledEvent** - 订单取消
   - 发布者: OrderService.CancelOrderAsync
   - 订阅者: OrderCancelledEventHandler
   - 行为: 更新取消统计、发送取消通知

8. **OrderStatusChangedEvent** - 订单状态变更
   - 发布者: OrderService.UpdateOrderStatusAsync
   - 订阅者: OrderStatusChangedEventHandler
   - 行为: 记录状态变更、发送状态通知

## 数据库设计

### 核心表结构

1. **Users** - 用户表
   - Id, UserName, Email, PasswordHash, Phone, Address
   - CreatedAt, UpdatedAt, IsActive

2. **Products** - 产品表
   - Id, Name, Description, Price, Stock, Category
   - ImageUrl, IsActive, CreatedAt, UpdatedAt

3. **Orders** - 订单表
   - Id, UserId, Status, TotalAmount, PaymentMethod
   - CreatedAt, UpdatedAt, ExpiresAt

4. **OrderItems** - 订单项表
   - Id, OrderId, ProductId, Quantity, UnitPrice

5. **ShoppingCarts** - 购物车表
   - Id, UserId, CreatedAt, UpdatedAt

6. **ShoppingCartItems** - 购物车项表
   - Id, ShoppingCartId, ProductId, Quantity

7. **InventoryTransactions** - 库存事务表
   - Id, ProductId, OrderId, Quantity, OperationType
   - Reason, CreatedAt

8. **OutboxMessages** - 发件箱表
   - Id, EventType, EventData, CreatedAt, ProcessedAt
   - ErrorMessage, RetryCount

9. **RefreshTokens** - 刷新令牌表
   - Id, UserId, Token, ExpiresAt, CreatedAt

## API 端点

### 认证相关
- `POST /api/auth/register` - 用户注册
- `POST /api/auth/login` - 用户登录
- `POST /api/auth/refresh` - 刷新令牌
- `POST /api/auth/logout` - 用户登出

### 用户相关
- `GET /api/users/profile` - 获取用户信息
- `PUT /api/users/profile` - 更新用户信息

### 产品相关
- `GET /api/products` - 获取产品列表
- `GET /api/products/{id}` - 获取产品详情
- `POST /api/products` - 创建产品
- `PUT /api/products/{id}` - 更新产品
- `DELETE /api/products/{id}` - 删除产品

### 订单相关
- `GET /api/orders` - 获取订单列表
- `GET /api/orders/{id}` - 获取订单详情
- `POST /api/orders` - 创建订单
- `PUT /api/orders/{id}/status` - 更新订单状态
- `POST /api/orders/{id}/cancel` - 取消订单

### 支付相关
- `POST /api/payment/process` - 处理支付
- `GET /api/payment/{id}/status` - 查询支付状态

### 库存相关
- `GET /api/inventory/products/{id}/stock` - 查询库存
- `POST /api/inventory/products/{id}/lock` - 锁定库存
- `POST /api/inventory/products/{id}/unlock` - 释放库存

### 购物车相关
- `GET /api/shopping-cart` - 获取购物车
- `POST /api/shopping-cart/items` - 添加商品到购物车
- `PUT /api/shopping-cart/items/{id}` - 更新购物车项
- `DELETE /api/shopping-cart/items/{id}` - 删除购物车项
- `DELETE /api/shopping-cart/clear` - 清空购物车

## 配置说明

### 数据库连接
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ECommerceDb;User=root;Password=123456;"
  }
}
```

### JWT 配置
```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key",
    "Issuer": "ECommerceAPI",
    "Audience": "ECommerceClient",
    "ExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

### RabbitMQ 配置
```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/"
  }
}
```

## 运行说明

### 环境要求
- .NET 9 SDK
- MySQL 8.0+
- RabbitMQ (可选)

### 数据库初始化

#### 方法一：使用批处理脚本（推荐）
1. 确保 MySQL 服务运行
2. 创建数据库: `CREATE DATABASE ECommerceDb;`
3. 运行初始化脚本: `init-database.bat`
4. 选择选项 1（迁移数据库）
5. 等待数据库创建完成

#### 方法二：使用命令行
```bash
cd ECommerce.API
dotnet run -- db-migrate
```

#### 方法三：检查数据库状态
```bash
cd ECommerce.API
dotnet run -- db-status
```

### 启动步骤
1. 完成数据库初始化
2. 运行项目: `dotnet run`
3. 访问 Swagger: `https://localhost:7037/swagger`

### 测试数据
系统启动时会自动创建测试数据：
- 管理员: admin@example.com / admin123
- 客户: customer1@example.com / customer123
- 测试产品: iPhone 15 Pro, MacBook Pro, Wireless Headphones

### 故障排除

#### 问题：运行 init-database.bat 选择1后没有创建表
**原因**: Program.cs 缺少对数据库初始化命令的支持

**解决方案**:
1. 确保 Program.cs 包含以下代码：
```csharp
// 检查是否是数据库初始化命令
if (args.Length > 0 && args[0].StartsWith("db-"))
{
    await RunDatabaseInitTool(args);
    return;
}
```

2. 确保添加了必要的 using 语句：
```csharp
using ECommerce.API;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
```

3. 确保 Program.cs 末尾包含 RunDatabaseInitTool 方法

#### 问题：编译错误 "未能找到类型或命名空间名 DatabaseInitTool"
**解决方案**: 添加 `using ECommerce.API;` 到 Program.cs 文件顶部

#### 问题：文件被锁定无法编译
**解决方案**: 
1. 停止所有正在运行的 ECommerce.API 进程
2. 运行 `dotnet clean`
3. 重新构建项目

#### 问题：数据库连接失败
**解决方案**:
1. 检查 MySQL 服务是否运行
2. 验证连接字符串配置
3. 确保数据库 ECommerceDb 已创建
4. 检查用户名和密码是否正确

### 数据库命令参考

系统提供了以下数据库管理命令：

| 命令 | 描述 | 示例 |
|------|------|------|
| `db-migrate` | 迁移数据库（创建表结构） | `dotnet run -- db-migrate` |
| `db-reset` | 重置数据库（删除并重新创建） | `dotnet run -- db-reset` |
| `db-seed` | 填充种子数据（如果数据库为空） | `dotnet run -- db-seed` |
| `db-force-seed` | 强制重新填充种子数据（删除现有种子数据） | `dotnet run -- db-force-seed` |
| `db-status` | 检查数据库状态 | `dotnet run -- db-status` |
| `db-help` | 显示帮助信息 | `dotnet run -- db-help` |

**注意事项**:
- 所有命令都需要在 `ECommerce.API` 目录下执行
- `db-reset` 命令会删除所有数据，请谨慎使用
- 首次运行建议使用 `db-migrate` 命令
- 种子数据现在只在数据库初始化时执行，不会在每次接口调用时重复执行
- 使用 `db-force-seed` 可以强制重新填充种子数据（会删除现有的种子数据）

## 开发指南

### 添加新功能
1. 在 Domain 层定义实体和事件
2. 在 Application 层实现业务逻辑
3. 在 Infrastructure 层实现数据访问
4. 在 API 层添加控制器端点

### 事件处理
1. 定义领域事件
2. 实现事件处理器
3. 在服务中发布事件
4. 注册事件处理器到 DI 容器

### 数据库操作
1. 修改实体模型
2. 更新 DbContext
3. 重启应用自动创建表结构

## 监控和日志

- 使用 ILogger 进行日志记录
- 事件处理异常会被记录
- 支持结构化日志输出
- 集成 Swagger 进行 API 测试

## 安全考虑

- JWT 令牌认证
- 密码哈希存储
- CORS 配置
- 全局异常处理
- 输入验证

## 性能优化

- 异步操作
- 数据库连接池
- 缓存机制
- 事件驱动架构
- 后台服务处理

## 扩展性

- 微服务架构设计
- 事件驱动解耦
- 依赖注入
- 接口抽象
- 插件化设计
