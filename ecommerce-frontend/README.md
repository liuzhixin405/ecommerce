# ECommerce Frontend

这是一个基于 React + TypeScript + Tailwind CSS 的电商前端应用。

## 功能特性

- ✅ 产品展示和搜索
- ✅ 购物车管理
- ✅ 响应式设计
- ✅ 现代化UI界面
- ✅ TypeScript 类型安全
- ✅ 与后端API集成

## 技术栈

- **React 19** - 前端框架
- **TypeScript** - 类型安全
- **Tailwind CSS** - 样式框架
- **Axios** - HTTP客户端
- **Lucide React** - 图标库
- **React Hook Form** - 表单管理

## 项目结构

```
src/
├── api/              # API配置和客户端
├── components/       # 可复用组件
│   ├── ui/          # 基础UI组件
│   └── ...          # 业务组件
├── interfaces/       # TypeScript接口定义
├── pages/           # 页面组件
├── services/        # API服务
├── utils/           # 工具函数
└── App.tsx          # 主应用组件
```

## 安装和运行

### 前置要求

- Node.js 18+
- npm 或 yarn

### 安装依赖

#### 方法1: 使用安装脚本 (推荐)
双击运行 `install.bat` 文件，它会自动处理安装过程。

#### 方法2: 手动安装

如果遇到 PowerShell 执行策略问题，请使用命令提示符 (CMD)：

```cmd
cd G:\ClaudeCode\ecommerce-frontend
npm install
```

#### 方法3: 使用 Git Bash
在项目目录右键选择 "Git Bash Here"，然后运行：
```bash
npm install
```

#### 如果遇到问题，请参考 [安装指南](INSTALLATION_GUIDE.md)

### API配置

前端默认连接到 `https://localhost:7037/api`。如需修改API地址，请参考 [API配置说明](API_CONFIG.md)。

### 启动开发服务器

```bash
npm start
```

应用将在 http://localhost:3000 启动

### 构建生产版本

```bash
npm run build
```

## 主要功能

### 产品管理
- 产品列表展示
- 产品搜索
- 分类筛选
- 产品详情

### 购物车
- 添加商品到购物车
- 修改商品数量
- 删除商品
- 清空购物车
- 计算总价

### 用户界面
- 响应式导航栏
- 购物车图标显示商品数量
- 现代化卡片设计
- 加载状态和错误处理

## API集成

前端与后端API完全集成，支持：

- 产品CRUD操作
- 订单管理
- 用户管理
- 购物车功能

## 开发指南

### 添加新组件

1. 在 `src/components/` 目录下创建新组件
2. 使用 TypeScript 定义接口
3. 使用 Tailwind CSS 进行样式设计
4. 添加必要的测试

### 添加新页面

1. 在 `src/pages/` 目录下创建新页面
2. 在 `App.tsx` 中添加路由
3. 更新导航栏

### 样式指南

- 使用 Tailwind CSS 类名
- 遵循响应式设计原则
- 保持一致的视觉风格
- 使用 `cn()` 工具函数组合类名

## 部署

### 构建

```bash
npm run build
```

### 部署到静态服务器

将 `build/` 目录部署到任何静态文件服务器。

### 环境变量

创建 `.env` 文件配置API地址：

```
REACT_APP_API_URL=https://localhost:7037/api
```

## 贡献

1. Fork 项目
2. 创建功能分支
3. 提交更改
4. 推送到分支
5. 创建 Pull Request

## 许可证

MIT License
