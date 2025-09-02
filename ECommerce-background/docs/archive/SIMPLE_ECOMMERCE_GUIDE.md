# 简化电商系统

一个简单易懂的电商系统，专注于核心业务功能。

## 🎯 系统特点

- ✅ **简单清晰** - 避免过度设计，专注于核心功能
- ✅ **易于理解** - 代码结构清晰，业务流程直观
- ✅ **快速上手** - 最小化配置，快速运行
- ✅ **完整功能** - 包含用户、产品、订单、库存管理
- ✅ **事件驱动** - 松耦合的事件处理机制

## 🏗️ 系统架构

### 分层结构
```
Controller (控制器) - 处理HTTP请求
    ↓
Service (服务层) - 业务逻辑
    ↓
Repository (仓储层) - 数据访问
    ↓
Database (数据库) - 数据存储
```

### 事件驱动架构
```
业务操作 → 发布事件 → 事件处理器 → 处理业务逻辑
```

### 核心实体
- **User** - 用户信息
- **Product** - 产品信息
- **Order** - 订单信息
- **OrderItem** - 订单项
- **Inventory** - 库存信息

## 🔄 业务流程

### 用户购物流程
```
1. 用户注册/登录
2. 浏览产品列表
3. 查看产品详情
4. 创建订单
5. 支付订单
6. 订单完成
```

### 订单处理流程
```
1. 创建订单 → 检查库存 → 锁定库存
2. 支付订单 → 扣减库存 → 更新订单状态
3. 发货 → 更新订单状态
4. 完成 → 订单结束
```

## 🚀 快速开始

### 环境要求
- .NET 9 SDK
- MySQL Server 8.0+
- Python 3.x 或 Node.js (前端)

### 1. 数据库设置

**重要：如果遇到数据库schema错误，请先修复数据库**

```sql
-- 连接到MySQL并执行：
USE ecommerce;
ALTER TABLE OutboxMessages ADD COLUMN ErrorMessage TEXT NULL COMMENT '错误信息';
```

详细修复指南请参考：`../archive/DATABASE_FIX_GUIDE.md`

### 2. 启动后端

```bash
# 进入API目录
cd ECommerce.API

# 运行项目
dotnet run
```

后端将在以下地址启动：
- HTTP: http://localhost:5227
- Swagger文档: http://localhost:5227/swagger

### 3. 启动前端

```bash
# 进入前端目录
cd frontend

# 使用Python启动
python -m http.server 3000

# 或使用Node.js
npx http-server -p 3000
```

前端地址：http://localhost:3000

## 📝 测试数据

### 测试用户
- 邮箱: `admin@example.com`
- 密码: `admin123`

### 测试产品
- iPhone 15 Pro - ¥999.99
- MacBook Pro - ¥1999.99
- Wireless Headphones - ¥199.99

## 🔧 核心API

### 用户相关
- `POST /api/auth/register` - 用户注册
- `POST /api/auth/login` - 用户登录

### 产品相关
- `GET /api/products` - 获取产品列表
- `GET /api/products/{id}` - 获取产品详情

### 订单相关
- `POST /api/orders` - 创建订单
- `GET /api/orders/{id}` - 获取订单详情
- `PUT /api/orders/{id}/status` - 更新订单状态

## 💡 设计原则

1. **简单优先** - 避免过度设计
2. **业务驱动** - 以业务需求为导向
3. **易于理解** - 代码结构清晰
4. **易于维护** - 减少复杂依赖
5. **事件驱动** - 松耦合的事件处理

## 📁 项目结构

```
ECommerce.API/           # Web API层
├── Controllers/         # HTTP控制器
└── Extensions/         # 扩展方法

ECommerce.Application/   # 应用服务层
├── Services/           # 业务服务
└── EventHandlers/      # 事件处理器

ECommerce.Domain/        # 领域层
├── Entities/           # 实体
├── Events/             # 领域事件
├── Interfaces/         # 仓储接口
└── Models/             # 数据模型

ECommerce.Infrastructure/ # 基础设施层
├── Data/               # 数据访问
├── EventBus/           # 事件总线
├── Repositories/       # 仓储实现
└── Services/           # 基础设施服务
```

## 🎯 下一步

1. 完善前端界面
2. 添加更多业务功能
3. 优化用户体验
4. 添加测试用例
