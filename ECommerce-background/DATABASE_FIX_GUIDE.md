# 数据库修复指南

## 问题描述
项目运行时遇到数据库schema不匹配错误：
```
MySqlException: Unknown column 'o.ErrorMessage' in 'field list'
```

## 原因分析
`OutboxMessages`表缺少`ErrorMessage`列，但代码中的`OutboxMessage`模型包含此属性。

## 解决方案

### 方法1：手动执行SQL（推荐）

1. 连接到MySQL数据库：
```bash
mysql -u root -p
```

2. 选择数据库：
```sql
USE ecommerce;
```

3. 添加ErrorMessage列：
```sql
ALTER TABLE OutboxMessages 
ADD COLUMN ErrorMessage TEXT NULL 
COMMENT '错误信息';
```

4. 验证列已添加：
```sql
DESCRIBE OutboxMessages;
```

### 方法2：使用提供的SQL脚本

1. 确保MySQL客户端在PATH中
2. 运行脚本：
```bash
mysql -u root -p ecommerce < fix_outbox_schema.sql
```

### 方法3：临时禁用Outbox服务

如果暂时无法修复数据库，可以临时禁用Outbox服务：

在`ECommerce.API/Program.cs`中注释掉以下行：
```csharp
// builder.Services.AddScoped<IOutboxService, OutboxService>();
// builder.Services.AddHostedService<OutboxProcessorService>();
```

## 验证修复

1. 重新启动项目：
```bash
cd ECommerce.API
dotnet run
```

2. 检查是否还有数据库错误

## 注意事项

- 修复数据库schema后，Outbox模式将正常工作
- Outbox模式用于确保事件可靠发布，是事件驱动架构的重要组成部分
- 建议在生产环境中使用数据库迁移工具（如EF Core Migrations）来管理schema变更
