-- 电商系统数据库创建脚本
-- 适用于 MySQL 8.0+

-- 1. 删除现有数据库（如果存在）
DROP DATABASE IF EXISTS ECommerceDb;

-- 2. 创建新数据库
CREATE DATABASE ECommerceDb 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

-- 3. 使用新数据库
USE ECommerceDb;

-- 4. 创建用户表
CREATE TABLE Users (
    Id CHAR(36) PRIMARY KEY,
    UserName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    FirstName VARCHAR(100),
    LastName VARCHAR(100),
    PhoneNumber VARCHAR(20),
    Address TEXT,
    Role VARCHAR(50) DEFAULT 'User',
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt DATETIME(6) NOT NULL,
    UpdatedAt DATETIME(6) NOT NULL,
    INDEX IX_Users_Email (Email)
);

-- 5. 创建产品表
CREATE TABLE Products (
    Id CHAR(36) PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    Description TEXT,
    Price DECIMAL(18,2) NOT NULL,
    Stock INT NOT NULL DEFAULT 0,
    Category VARCHAR(100),
    ImageUrl VARCHAR(500),
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt DATETIME(6) NOT NULL,
    UpdatedAt DATETIME(6) NOT NULL,
    INDEX IX_Products_Category (Category),
    INDEX IX_Products_IsActive (IsActive)
);

-- 6. 创建订单表
CREATE TABLE Orders (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    Status VARCHAR(50) NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    PaymentMethod VARCHAR(50),
    PaymentId VARCHAR(100),
    CreatedAt DATETIME(6) NOT NULL,
    UpdatedAt DATETIME(6) NOT NULL,
    ExpiresAt DATETIME(6),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    INDEX IX_Orders_UserId (UserId),
    INDEX IX_Orders_Status (Status),
    INDEX IX_Orders_CreatedAt (CreatedAt)
);

-- 7. 创建订单项表
CREATE TABLE OrderItems (
    Id CHAR(36) PRIMARY KEY,
    OrderId CHAR(36) NOT NULL,
    ProductId CHAR(36) NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE RESTRICT,
    INDEX IX_OrderItems_OrderId (OrderId),
    INDEX IX_OrderItems_ProductId (ProductId)
);

-- 8. 创建刷新令牌表
CREATE TABLE RefreshTokens (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    Token VARCHAR(500) NOT NULL UNIQUE,
    ExpiresAt DATETIME(6) NOT NULL,
    CreatedAt DATETIME(6) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    INDEX IX_RefreshTokens_UserId (UserId),
    INDEX IX_RefreshTokens_Token (Token)
);

-- 9. 创建发件箱表
CREATE TABLE OutboxMessages (
    Id CHAR(36) PRIMARY KEY,
    EventType VARCHAR(200) NOT NULL,
    EventData LONGTEXT NOT NULL,
    Status VARCHAR(50) NOT NULL,
    CreatedAt DATETIME(6) NOT NULL,
    ProcessedAt DATETIME(6),
    ErrorMessage TEXT,
    RetryCount INT DEFAULT 0,
    INDEX IX_OutboxMessages_Status (Status),
    INDEX IX_OutboxMessages_CreatedAt (CreatedAt)
);

-- 10. 创建库存事务表
CREATE TABLE InventoryTransactions (
    Id CHAR(36) PRIMARY KEY,
    ProductId CHAR(36) NOT NULL,
    OrderId CHAR(36),
    Quantity INT NOT NULL,
    OperationType VARCHAR(50) NOT NULL,
    Reason VARCHAR(200),
    Notes TEXT,
    CreatedBy VARCHAR(100),
    CreatedAt DATETIME(6) NOT NULL,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
    INDEX IX_InventoryTransactions_ProductId (ProductId),
    INDEX IX_InventoryTransactions_OrderId (OrderId),
    INDEX IX_InventoryTransactions_CreatedAt (CreatedAt)
);

-- 11. 创建购物车表
CREATE TABLE ShoppingCarts (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    CreatedAt DATETIME(6) NOT NULL,
    UpdatedAt DATETIME(6) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    INDEX IX_ShoppingCarts_UserId (UserId)
);

-- 12. 创建购物车项表
CREATE TABLE ShoppingCartItems (
    Id CHAR(36) PRIMARY KEY,
    ShoppingCartId CHAR(36) NOT NULL,
    ProductId CHAR(36) NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME(6) NOT NULL,
    UpdatedAt DATETIME(6) NOT NULL,
    FOREIGN KEY (ShoppingCartId) REFERENCES ShoppingCarts(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
    INDEX IX_ShoppingCartItems_ShoppingCartId (ShoppingCartId),
    INDEX IX_ShoppingCartItems_ProductId (ProductId)
);

-- 13. 插入测试用户数据
INSERT INTO Users (Id, UserName, Email, PasswordHash, FirstName, LastName, PhoneNumber, Address, Role, IsActive, CreatedAt, UpdatedAt) VALUES
('11111111-1111-1111-1111-111111111111', 'admin', 'admin@example.com', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Admin', 'User', '+86-138-0000-0000', '北京市朝阳区某某街道123号', 'Admin', TRUE, NOW(6), NOW(6)),
('22222222-2222-2222-2222-222222222222', 'customer1', 'customer1@example.com', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'John', 'Doe', '+86-139-0000-0000', '上海市浦东新区某某路456号', 'User', TRUE, NOW(6), NOW(6));

-- 14. 插入测试产品数据
INSERT INTO Products (Id, Name, Description, Price, Stock, Category, ImageUrl, IsActive, CreatedAt, UpdatedAt) VALUES
('33333333-3333-3333-3333-333333333333', 'iPhone 15 Pro', 'Latest iPhone with advanced features', 999.99, 50, 'Electronics', 'https://example.com/iphone15.jpg', TRUE, NOW(6), NOW(6)),
('44444444-4444-4444-4444-444444444444', 'MacBook Pro', 'Powerful laptop for professionals', 1999.99, 25, 'Electronics', 'https://example.com/macbook.jpg', TRUE, NOW(6), NOW(6)),
('55555555-5555-5555-5555-555555555555', 'Wireless Headphones', 'High-quality wireless headphones', 199.99, 100, 'Electronics', 'https://example.com/headphones.jpg', TRUE, NOW(6), NOW(6));

-- 15. 显示创建结果
SELECT 'Database ECommerceDb created successfully' AS Status;
SELECT COUNT(*) AS UserCount FROM Users;
SELECT COUNT(*) AS ProductCount FROM Products;
