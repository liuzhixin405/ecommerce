# ECommerce API

这是一个基于.NET 9的电商API项目，采用领域驱动设计(DDD)和事件驱动架构(EDA)。

## 项目架构

### 分层架构
- **ECommerce.API**: Web API层，处理HTTP请求
- **ECommerce.Application**: 应用服务层，包含业务逻辑和事件处理器
- **ECommerce.Domain**: 领域层，包含实体、值对象、领域事件和仓储接口
- **ECommerce.Infrastructure**: 基础设施层，包含数据访问、仓储实现和事件总线
- **ECommerce.Core**: 核心层，包含共享接口和工具

### 主要特性
- ✅ 领域驱动设计(DDD)
- ✅ 事件驱动架构(EDA)
- ✅ EF Core MySQL数据库
- ✅ 依赖注入
- ✅ Swagger API文档
- ✅ 密码加密(BCrypt)
- ✅ 完整的CRUD操作
- ✅ 事件发布和订阅

## 实体模型

### User (用户)
- 基本信息：用户名、邮箱、密码
- 个人信息：姓名、电话、地址
- 状态管理：活跃状态、最后登录时间

### Product (产品)
- 基本信息：名称、描述、价格
- 库存管理：库存数量、分类
- 状态管理：活跃状态、图片URL

### Order (订单)
- 订单信息：用户、总金额、状态
- 配送信息：地址、电话、客户姓名
- 订单项：产品、数量、价格

## 事件系统

### 领域事件
- `OrderCreatedEvent`: 订单创建事件
- `OrderStatusChangedEvent`: 订单状态变更事件

### 事件处理器
- `OrderCreatedEventHandler`: 处理订单创建事件
- `OrderStatusChangedEventHandler`: 处理订单状态变更事件

## 数据库设置

本项目使用 MySQL 数据库。请参考 [MySQL 设置指南](MYSQL_SETUP.md) 进行数据库配置。

## CORS配置

前端应用运行在 `http://localhost:3000`，后端API运行在 `https://localhost:7037`。已配置CORS策略允许跨域请求。

详细配置请参考 [CORS设置指南](CORS_SETUP.md)。

### 快速开始
1. 安装 MySQL Server
2. 配置连接字符串 (appsettings.json)
3. 运行应用程序，数据库会自动创建

## API端点

### 产品管理
- `GET /api/products` - 获取所有产品
- `GET /api/products/{id}` - 获取指定产品
- `GET /api/products/category/{category}` - 按分类获取产品
- `GET /api/products/search?q={query}` - 搜索产品
- `POST /api/products` - 创建产品
- `PUT /api/products/{id}` - 更新产品
- `DELETE /api/products/{id}` - 删除产品

### 订单管理
- `GET /api/orders` - 获取所有订单
- `GET /api/orders/{id}` - 获取指定订单
- `GET /api/orders/user/{userId}` - 获取用户订单
- `GET /api/orders/status/{status}` - 按状态获取订单
- `POST /api/orders` - 创建订单
- `PUT /api/orders/{id}/status` - 更新订单状态
- `DELETE /api/orders/{id}` - 删除订单

### 用户管理
- `GET /api/users` - 获取所有用户
- `GET /api/users/{id}` - 获取指定用户
- `GET /api/users/email/{email}` - 按邮箱获取用户
- `POST /api/users` - 创建用户
- `PUT /api/users/{id}` - 更新用户
- `DELETE /api/users/{id}` - 删除用户
- `POST /api/users/login` - 用户登录

## 运行项目

1. 确保已安装.NET 9 SDK
2. 在项目根目录运行：
   ```bash
   dotnet restore
   dotnet build
   cd ECommerce.API
   dotnet run
   ```
3. 访问Swagger文档：`https://localhost:7001/swagger`

## 测试数据

项目启动时会自动创建测试数据：
- 管理员用户：admin@example.com / admin123
- 客户用户：customer1@example.com / customer123
- 示例产品：iPhone 15 Pro、MacBook Pro、Wireless Headphones

## 技术栈

- **.NET 9**
- **Entity Framework Core (In-Memory)**
- **ASP.NET Core Web API**
- **Swagger/OpenAPI**
- **BCrypt.Net-Next**
- **依赖注入容器**

## 设计模式

- **仓储模式**: 数据访问抽象
- **服务模式**: 业务逻辑封装
- **事件驱动**: 松耦合的事件处理
- **依赖注入**: 控制反转
- **DTO模式**: 数据传输对象
