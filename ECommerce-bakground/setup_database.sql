-- ECommerce Database Setup Script
-- This script creates the database and all necessary tables

-- Create database if it doesn't exist
CREATE DATABASE IF NOT EXISTS ECommerceDb;
USE ECommerceDb;

-- Create Users table
CREATE TABLE IF NOT EXISTS `Users` (
    `Id` char(36) NOT NULL,
    `UserName` varchar(50) NOT NULL,
    `Email` varchar(100) NOT NULL,
    `PasswordHash` varchar(100) NOT NULL,
    `FirstName` varchar(50) NULL,
    `LastName` varchar(50) NULL,
    `PhoneNumber` varchar(20) NULL,
    `Address` varchar(200) NULL,
    `IsActive` bit(1) NOT NULL DEFAULT 1,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    `LastLoginAt` datetime(6) NULL,
    `Role` varchar(20) NOT NULL DEFAULT 'User',
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_Users_Email` (`Email`)
);

-- Create Products table
CREATE TABLE IF NOT EXISTS `Products` (
    `Id` char(36) NOT NULL,
    `Name` varchar(200) NOT NULL,
    `Description` varchar(1000) NULL,
    `Price` decimal(18,2) NOT NULL,
    `Stock` int NOT NULL,
    `Category` varchar(100) NULL,
    `ImageUrl` varchar(500) NULL,
    `IsActive` bit(1) NOT NULL DEFAULT 1,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`)
);

-- Create Orders table
CREATE TABLE IF NOT EXISTS `Orders` (
    `Id` char(36) NOT NULL,
    `UserId` char(36) NOT NULL,
    `TotalAmount` decimal(18,2) NOT NULL,
    `Status` int NOT NULL DEFAULT 0,
    `ShippingAddress` varchar(200) NULL,
    `PhoneNumber` varchar(50) NULL,
    `CustomerName` varchar(100) NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    `PaidAt` datetime(6) NULL,
    `ShippedAt` datetime(6) NULL,
    `DeliveredAt` datetime(6) NULL,
    `CancelledAt` datetime(6) NULL,
    `PaymentMethod` varchar(100) NULL,
    `TrackingNumber` varchar(100) NULL,
    `Notes` varchar(500) NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Orders_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
);

-- Create OrderItems table
CREATE TABLE IF NOT EXISTS `OrderItems` (
    `Id` char(36) NOT NULL,
    `OrderId` char(36) NOT NULL,
    `ProductId` char(36) NOT NULL,
    `Quantity` int NOT NULL,
    `Price` decimal(18,2) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_OrderItems_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_OrderItems_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE RESTRICT
);

-- Create RefreshTokens table
CREATE TABLE IF NOT EXISTS `RefreshTokens` (
    `Id` char(36) NOT NULL,
    `UserId` char(36) NOT NULL,
    `Token` varchar(500) NOT NULL,
    `ExpiresAt` datetime(6) NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `IsRevoked` bit(1) NOT NULL DEFAULT 0,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_RefreshTokens_Token` (`Token`),
    CONSTRAINT `FK_RefreshTokens_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
);

-- Insert seed data for Users (password is 'password')
INSERT INTO `Users` (`Id`, `UserName`, `Email`, `PasswordHash`, `FirstName`, `LastName`, `Role`, `IsActive`, `CreatedAt`, `UpdatedAt`) VALUES
('11111111-1111-1111-1111-111111111111', 'admin', 'admin@example.com', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'Admin', 'User', 'Admin', 1, NOW(), NOW()),
('22222222-2222-2222-2222-222222222222', 'customer1', 'customer1@example.com', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'John', 'Doe', 'User', 1, NOW(), NOW());

-- Insert seed data for Products
INSERT INTO `Products` (`Id`, `Name`, `Description`, `Price`, `Stock`, `Category`, `ImageUrl`, `IsActive`, `CreatedAt`, `UpdatedAt`) VALUES
('33333333-3333-3333-3333-333333333333', 'iPhone 15 Pro', 'Latest iPhone with advanced features', 999.99, 50, 'Electronics', 'https://example.com/iphone15.jpg', 1, NOW(), NOW()),
('44444444-4444-4444-4444-444444444444', 'MacBook Pro', 'Powerful laptop for professionals', 1999.99, 25, 'Electronics', 'https://example.com/macbook.jpg', 1, NOW(), NOW()),
('55555555-5555-5555-5555-555555555555', 'Wireless Headphones', 'High-quality wireless headphones', 199.99, 100, 'Electronics', 'https://example.com/headphones.jpg', 1, NOW(), NOW());

-- Show created tables
SHOW TABLES;

-- Show table structures
DESCRIBE Users;
DESCRIBE Products;
DESCRIBE Orders;
DESCRIBE OrderItems;
DESCRIBE RefreshTokens;
