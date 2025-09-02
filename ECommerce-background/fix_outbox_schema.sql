-- 修复OutboxMessages表，添加ErrorMessage列
USE ecommerce;

-- 检查ErrorMessage列是否存在
SELECT COUNT(*) as column_exists 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = 'ecommerce' 
  AND TABLE_NAME = 'OutboxMessages' 
  AND COLUMN_NAME = 'ErrorMessage';

-- 如果列不存在，则添加
ALTER TABLE OutboxMessages 
ADD COLUMN ErrorMessage TEXT NULL 
COMMENT '错误信息';

-- 验证列已添加
DESCRIBE OutboxMessages;
