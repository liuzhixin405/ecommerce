-- 数据库重置脚本
-- 解决种子数据中缺失字段值的问题

-- 1. 删除现有数据库
DROP DATABASE IF EXISTS ECommerceDb;

-- 2. 重新创建数据库
CREATE DATABASE ECommerceDb;

-- 3. 使用新数据库
USE ECommerceDb;

-- 4. 验证数据库创建成功
SELECT 'Database ECommerceDb created successfully' AS Status;

-- 注意：重启后端服务后，Entity Framework会自动：
-- - 创建所有表结构
-- - 插入修复后的种子数据
-- - 包含完整的用户信息（包括电话号码和地址）
