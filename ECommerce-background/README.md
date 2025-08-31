# ECommerce API

这是一个基于.NET 9的电商API项目，采用领域驱动设计(DDD)和事件驱动架构(EDA)。

## 🎯 项目特点

- ✅ **清晰的架构**: 分层架构，职责明确
- ✅ **事件驱动**: 松耦合的事件处理机制
- ✅ **简化实现**: EventHandlers专注核心业务逻辑
- ✅ **完整功能**: 用户、产品、订单、库存、支付管理
- ✅ **易于扩展**: 模块化设计，便于添加新功能

## 🏗️ 项目架构

### 分层架构
- **ECommerce.API**: Web API层，处理HTTP请求
- **ECommerce.Application**: 应用服务层，包含业务逻辑和事件处理器
- **ECommerce.Domain**: 领域层，包含实体、领域事件和仓储接口
- **ECommerce.Infrastructure**: 基础设施层，包含数据访问、仓储实现和事件总线
- **ECommerce.Core**: 核心层，包含共享接口和工具

### 核心实体
- **User**: 用户管理
- **Product**: 产品管理
- **Order**: 订单管理
- **Inventory**: 库存管理

## 🔄 业务流程

### 1. 用户注册
```
用户注册 → 创建用户 → 发布UserRegisteredEvent → 处理注册确认 → 完成
```

### 2. 下单流程
```
用户下单 → 检查库存 → 锁定库存 → 创建订单 → 发布OrderCreatedEvent → 处理订单确认 → 完成
```

### 3. 支付流程
```
用户支付 → 处理支付 → 更新订单状态 → 发布OrderPaidEvent → 处理支付确认 → 完成
```

### 4. 库存管理
```
库存变更 → 发布InventoryUpdatedEvent → 处理库存更新 → 检查低库存警告 → 完成
```

## 🚀 快速开始

### 环境要求
- .NET 9 SDK
- MySQL Server
- RabbitMQ (可选，用于事件总线)

### 安装步骤
1. 克隆项目
2. 配置数据库连接字符串 (`appsettings.json`)
3. 运行数据库迁移
4. 启动应用程序

```bash
dotnet restore
dotnet build
cd ECommerce.API
dotnet run
```

### 访问API文档
启动后访问: `https://localhost:7001/swagger`

## 📡 API端点

### 用户管理
- `POST /api/users` - 用户注册
- `POST /api/users/login` - 用户登录
- `GET /api/users/{id}` - 获取用户信息

### 产品管理
- `GET /api/products` - 获取产品列表
- `POST /api/products` - 创建产品
- `PUT /api/products/{id}` - 更新产品
- `DELETE /api/products/{id}` - 删除产品

### 订单管理
- `POST /api/orders` - 创建订单
- `GET /api/orders/{id}` - 获取订单详情
- `PUT /api/orders/{id}/status` - 更新订单状态

### 库存管理
- `GET /api/inventory/{productId}` - 获取库存信息
- `POST /api/inventory/transactions` - 创建库存交易

## 🔧 配置说明

### 数据库配置
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ecommerce;Uid=root;Pwd=password;"
  }
}
```

### 事件总线配置
```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest"
  }
}
```

## 🧪 测试数据

项目启动时会自动创建测试数据：
- 管理员用户: `admin@example.com` / `admin123`
- 客户用户: `customer1@example.com` / `customer123`
- 示例产品: iPhone 15 Pro、MacBook Pro、Wireless Headphones

## 📚 技术栈

- **.NET 9** - 最新版本的.NET框架
- **Entity Framework Core** - 数据访问ORM
- **MySQL** - 关系型数据库
- **RabbitMQ** - 消息队列和事件总线
- **Swagger/OpenAPI** - API文档
- **依赖注入** - 控制反转容器

## 🎨 设计模式

- **仓储模式**: 数据访问抽象
- **服务模式**: 业务逻辑封装
- **事件驱动**: 松耦合的事件处理
- **依赖注入**: 控制反转
- **CQRS**: 命令查询职责分离

## 📖 更多文档

- [业务流程说明](BUSINESS_FLOW.md) - 详细的业务流程说明
- [项目清理总结](PROJECT_CLEANUP_SUMMARY.md) - 项目重构和清理总结
- [MySQL设置指南](MYSQL_SETUP.md) - 数据库配置说明
- [CORS设置指南](CORS_SETUP.md) - 跨域配置说明

## 🤝 贡献

欢迎提交Issue和Pull Request来改进这个项目。

## �� 许可证

本项目采用MIT许可证。
