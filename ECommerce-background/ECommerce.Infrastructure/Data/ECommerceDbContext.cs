using ECommerce.Domain.Entities;
using ECommerce.Domain.Models;
using Microsoft.EntityFrameworkCore;
using static BCrypt.Net.BCrypt;

namespace ECommerce.Infrastructure.Data
{
    public class ECommerceDbContext : DbContext
    {
        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            });

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Price);
                entity.Property(e => e.Stock);
            });

            // Order configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount);
                entity.HasOne(e => e.User)
                    .WithMany(e => e.Orders)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // OrderItem configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price);
                entity.HasOne(e => e.Order)
                    .WithMany(e => e.Items)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Product)
                    .WithMany(e => e.OrderItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // RefreshToken configuration
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
                entity.HasIndex(e => e.Token).IsUnique();
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // OutboxMessage configuration
            modelBuilder.Entity<OutboxMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EventType).IsRequired().HasMaxLength(200);
                entity.Property(e => e.EventData).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.RetryCount).HasDefaultValue(0);
                
                // 索引优化
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);
            });

            // InventoryTransaction configuration
            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Reason).HasMaxLength(200);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.HasOne<Product>()
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ShoppingCart configuration
            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User);
                    //.WithMany(e => e.ShoppingCarts)
                    //.HasForeignKey(e => e.UserId)
                    //.OnDelete(DeleteBehavior.Cascade);
            });

            // ShoppingCartItem configuration
            modelBuilder.Entity<ShoppingCartItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.HasOne(e => e.ShoppingCart)
                    .WithMany(e => e.Items)
                    .HasForeignKey(e => e.ShoppingCartId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Users
            var users = new List<User>
            {
                new User
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    UserName = "admin",
                    Email = "admin@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    FirstName = "Admin",
                    LastName = "User",
                    PhoneNumber = "+86-138-0000-0000",
                    Address = "北京市朝阳区某某街道123号",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    UserName = "customer1",
                    Email = "customer1@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("customer123"),
                    FirstName = "John",
                    LastName = "Doe",
                    PhoneNumber = "+86-139-0000-0000",
                    Address = "上海市浦东新区某某路456号",
                    Role = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            // Seed Products
            var products = new List<Product>
            {
                new Product
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "iPhone 15 Pro",
                    Description = "Latest iPhone with advanced features",
                    Price = 999.99m,
                    Stock = 50,
                    Category = "Electronics",
                    ImageUrl = "https://example.com/iphone15.jpg",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Name = "MacBook Pro",
                    Description = "Powerful laptop for professionals",
                    Price = 1999.99m,
                    Stock = 25,
                    Category = "Electronics",
                    ImageUrl = "https://example.com/macbook.jpg",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Name = "Wireless Headphones",
                    Description = "High-quality wireless headphones",
                    Price = 199.99m,
                    Stock = 100,
                    Category = "Electronics",
                    ImageUrl = "https://example.com/headphones.jpg",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            modelBuilder.Entity<User>().HasData(users);
            modelBuilder.Entity<Product>().HasData(products);
        }
    }
}
