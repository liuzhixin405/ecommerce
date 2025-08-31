# ECommerce 电商系统

这是一个完整的电商系统，包含前端React应用和后端.NET Core API，支持用户认证、权限管理和商品管理。

## 功能特性

### 后端 (.NET Core API)
- ✅ JWT认证和授权
- ✅ 用户管理 (CRUD操作)
- ✅ 角色权限控制 (Admin/User)
- ✅ 商品管理
- ✅ 订单管理
- ✅ MySQL数据库集成
- ✅ Swagger API文档

### 前端 (React + TypeScript)
- ✅ 用户登录/登出
- ✅ 用户管理界面 (管理员)
- ✅ 商品展示
- ✅ 购物车功能
- ✅ 响应式设计

## 技术栈

### 后端
- .NET 9.0
- Entity Framework Core
- MySQL
- JWT Bearer Authentication
- Swagger/OpenAPI

### 前端
- React 18
- TypeScript
- Tailwind CSS
- Lucide React Icons

## 快速开始

### 1. 环境要求
- .NET 9.0 SDK
- Node.js 16+
- MySQL 8.0+

### 2. 数据库设置
1. 创建MySQL数据库：
```sql
CREATE DATABASE ECommerceDb;
```

2. 更新连接字符串：
编辑 `ECommerceApp/ECommerce.API/appsettings.json` 中的数据库连接字符串。

### 3. 启动应用

#### 方法一：使用启动脚本
```bash
# Windows
start-all.bat

# 或者手动启动
```

#### 方法二：手动启动

**启动后端：**
```bash
cd ECommerceApp
dotnet run --project ECommerce.API
```

**启动前端：**
```bash
cd ecommerce-frontend
npm install
npm start
```

### 4. 访问应用
- 前端: http://localhost:3000
- 后端API: http://localhost:5000
- Swagger文档: http://localhost:5000/swagger

## 测试账号

### 管理员账号
- 邮箱: admin@example.com
- 密码: admin123
- 权限: 可以访问用户管理功能

### 普通用户账号
- 邮箱: customer1@example.com
- 密码: customer123
- 权限: 只能访问商品和购物车功能

## API 端点

### 认证相关
- `POST /api/auth/login` - 用户登录
- `POST /api/auth/refresh` - 刷新Token
- `POST /api/auth/logout` - 用户登出

### 用户管理
- `GET /api/users` - 获取所有用户 (需要Admin权限)
- `GET /api/users/{id}` - 获取指定用户
- `POST /api/users` - 创建用户
- `PUT /api/users/{id}` - 更新用户
- `DELETE /api/users/{id}` - 删除用户

### 商品管理
- `GET /api/products` - 获取所有商品
- `GET /api/products/{id}` - 获取指定商品

### 订单管理
- `GET /api/orders` - 获取所有订单
- `POST /api/orders` - 创建订单

## 项目结构

```
├── ECommerceApp/                 # 后端项目
│   ├── ECommerce.API/           # API层
│   ├── ECommerce.Application/   # 应用服务层
│   ├── ECommerce.Domain/        # 领域层
│   └── ECommerce.Infrastructure/# 基础设施层
├── ecommerce-frontend/          # 前端项目
│   ├── src/
│   │   ├── components/         # React组件
│   │   ├── services/          # API服务
│   │   ├── contexts/          # React上下文
│   │   └── interfaces/        # TypeScript接口
└── start-all.bat              # 启动脚本
```

## 开发说明

### 添加新的API端点
1. 在 `ECommerce.Domain/Models` 中添加DTO
2. 在 `ECommerce.Application/Services` 中添加服务
3. 在 `ECommerce.API/Controllers` 中添加控制器

### 添加新的前端功能
1. 在 `src/interfaces` 中定义TypeScript接口
2. 在 `src/services` 中添加API调用
3. 在 `src/components` 中创建React组件

## 部署

### 后端部署
```bash
cd ECommerceApp
dotnet publish -c Release
```

### 前端部署
```bash
cd ecommerce-frontend
npm run build
```

## 许可证

MIT License
