# 简化电商系统 - 核心业务流程

## 🎯 系统目标
一个简单易懂的电商系统，专注于核心业务功能。

## 📋 核心功能

### 1. 用户管理
- 用户注册
- 用户登录
- 用户信息管理

### 2. 产品管理
- 产品列表
- 产品详情
- 产品搜索

### 3. 订单管理
- 创建订单
- 订单状态管理
- 订单历史

### 4. 库存管理
- 库存查询
- 库存更新

## 🔄 业务流程

### 用户购物流程
```
1. 用户注册/登录
2. 浏览产品列表
3. 查看产品详情
4. 添加到购物车
5. 创建订单
6. 支付订单
7. 订单完成
```

### 订单处理流程
```
1. 创建订单 → 检查库存 → 锁定库存
2. 支付订单 → 扣减库存 → 更新订单状态
3. 发货 → 更新订单状态
4. 完成 → 订单结束
```

## 🏗️ 简化架构

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

### 核心实体
- **User** - 用户信息
- **Product** - 产品信息
- **Order** - 订单信息
- **OrderItem** - 订单项
- **Inventory** - 库存信息

## 🚀 快速开始

### 1. 启动后端
```bash
cd ECommerce.API
dotnet run
```

### 2. 启动前端
```bash
cd frontend
python -m http.server 3000
```

### 3. 访问系统
- 前端: http://localhost:3000
- API文档: http://localhost:5227/swagger

## 📝 测试数据

### 测试用户
- 邮箱: admin@example.com
- 密码: admin123

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

## 🎯 下一步

1. 完善前端界面
2. 添加更多业务功能
3. 优化用户体验
4. 添加测试用例
