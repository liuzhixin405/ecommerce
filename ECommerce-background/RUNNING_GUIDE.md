# 电商系统运行指南

## 环境要求

### 后端要求
- .NET 9 SDK
- MySQL Server 8.0+
- RabbitMQ (可选，用于事件总线)

### 前端要求
- Python 3.x 或 Node.js
- 现代浏览器

## 快速启动

### 1. 启动后端

#### 方法1: 使用批处理脚本
```bash
# Windows
start-backend.bat

# 或手动执行
cd ECommerce.API
dotnet run
```

#### 方法2: 手动启动
```bash
# 还原包
dotnet restore

# 构建项目
dotnet build

# 启动API
cd ECommerce.API
dotnet run
```

后端将在以下地址启动：
- HTTPS: https://localhost:7037
- HTTP: http://localhost:5037
- Swagger文档: https://localhost:7037/swagger

### 2. 启动前端

#### 方法1: 使用批处理脚本
```bash
# Windows
start-frontend.bat
```

#### 方法2: 手动启动
```bash
cd frontend

# 使用Python
python -m http.server 3000

# 或使用Node.js
npx http-server -p 3000
```

前端将在以下地址启动：
- http://localhost:3000

## 数据库配置

### 1. MySQL设置
确保MySQL服务正在运行，并创建数据库：

```sql
CREATE DATABASE ECommerceDb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

### 2. 连接字符串
在 `ECommerce.API/appsettings.json` 中配置：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ECommerceDb;User=root;Password=123456;"
  }
}
```

### 3. 数据库初始化
项目启动时会自动创建表结构和测试数据：

**测试用户:**
- 管理员: admin@example.com / admin123
- 客户: customer1@example.com / customer123

**测试产品:**
- iPhone 15 Pro - ¥999.99
- MacBook Pro - ¥1999.99
- Wireless Headphones - ¥199.99

## 功能测试

### 1. 用户注册
```bash
POST /api/auth/register
{
  "userName": "testuser",
  "email": "test@example.com",
  "password": "password123"
}
```

### 2. 用户登录
```bash
POST /api/auth/login
{
  "email": "test@example.com",
  "password": "password123"
}
```

### 3. 获取产品列表
```bash
GET /api/products
```

### 4. 创建订单
```bash
POST /api/orders
Authorization: Bearer {token}
{
  "items": [
    {
      "productId": "33333333-3333-3333-3333-333333333333",
      "quantity": 1
    }
  ]
}
```

## 故障排除

### 常见问题

#### 1. 数据库连接失败
- 检查MySQL服务是否运行
- 验证连接字符串中的用户名和密码
- 确保数据库已创建

#### 2. 端口被占用
- 检查端口7037和3000是否被占用
- 可以在配置文件中修改端口

#### 3. 编译错误
- 确保安装了.NET 9 SDK
- 运行 `dotnet restore` 还原包
- 检查是否有语法错误

#### 4. 前端无法访问后端
- 检查CORS配置
- 确保后端正在运行
- 检查网络连接

### 日志查看
- 后端日志在控制台输出
- 前端错误在浏览器控制台查看

## 开发模式

### 热重载
- 后端: 修改代码后自动重启
- 前端: 刷新浏览器即可

### 调试
- 后端: 使用Visual Studio或VS Code调试
- 前端: 使用浏览器开发者工具

## 生产部署

### 后端部署
1. 发布项目: `dotnet publish -c Release`
2. 配置生产环境变量
3. 使用反向代理(如Nginx)
4. 配置HTTPS证书

### 前端部署
1. 构建静态文件
2. 部署到Web服务器
3. 配置CDN加速

## 联系支持

如遇到问题，请检查：
1. 环境配置是否正确
2. 依赖服务是否运行
3. 日志输出中的错误信息
4. 网络连接是否正常
