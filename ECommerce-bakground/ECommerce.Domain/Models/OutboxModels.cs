using System.ComponentModel.DataAnnotations;

namespace ECommerce.Domain.Models
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        
        [Required]
        public string Type { get; set; } = string.Empty;
        
        [Required]
        public string Data { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? ProcessedAt { get; set; }
        
        public string? Error { get; set; }
        
        public int RetryCount { get; set; }
        
        public DateTime? NextRetryAt { get; set; }
        
        public OutboxMessageStatus Status { get; set; }
        
        public string? CorrelationId { get; set; }
        
        public string? CausationId { get; set; }
    }

    public enum OutboxMessageStatus
    {
        Pending = 0,
        Processing = 1,
        Completed = 2,
        Failed = 3,
        Retry = 4
    }

    public interface IOutboxMessage
    {
        Guid Id { get; }
        string Type { get; }
        string Data { get; }
        DateTime CreatedAt { get; }
        string? CorrelationId { get; }
        string? CausationId { get; }
    }

    public abstract class DomainEvent : IOutboxMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Type => GetType().Name;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CorrelationId { get; set; }
        public string? CausationId { get; set; }
        
        public abstract string Data { get; }
    }

    // 具体的事件类型
    public class OrderCreatedEvent : DomainEvent
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        
        public override string Data => System.Text.Json.JsonSerializer.Serialize(this);
    }

    public class OrderPaidEvent : DomainEvent
    {
        public Guid OrderId { get; set; }
        public string PaymentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        
        public override string Data => System.Text.Json.JsonSerializer.Serialize(this);
    }

    public class OrderCancelledEvent : DomainEvent
    {
        public Guid OrderId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime CancelledAt { get; set; }
        
        public override string Data => System.Text.Json.JsonSerializer.Serialize(this);
    }

    public class InventoryUpdatedEvent : DomainEvent
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int OldStock { get; set; }
        public int NewStock { get; set; }
        public string OperationType { get; set; } = string.Empty;
        public Guid? OrderId { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public override string Data => System.Text.Json.JsonSerializer.Serialize(this);
    }

    public class PaymentProcessedEvent : DomainEvent
    {
        public string PaymentId { get; set; } = string.Empty;
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime ProcessedAt { get; set; }
        
        public override string Data => System.Text.Json.JsonSerializer.Serialize(this);
    }

    public class StockLockedEvent : DomainEvent
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public Guid OrderId { get; set; }
        public DateTime LockedAt { get; set; }
        
        public override string Data => System.Text.Json.JsonSerializer.Serialize(this);
    }

    public class StockReleasedEvent : DomainEvent
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public Guid OrderId { get; set; }
        public DateTime ReleasedAt { get; set; }
        
        public override string Data => System.Text.Json.JsonSerializer.Serialize(this);
    }

    // 新增事件类型：完善电商业务流程
    public class UserRegisteredEvent : DomainEvent
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
        
        public override string Data => System.Text.Json.JsonSerializer.Serialize(this);
    }

    public class UserLoggedInEvent : DomainEvent
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime LoggedInAt { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        
        public override string Data => System.Text.Json.JsonSerializer.Serialize(this);
    }

    public class ProductViewedEvent : DomainEvent
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime ViewedAt { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        
        public override string Data => System.Text.Json.JsonSerializer.Serialize(this);
    }

    public class CartUpdatedEvent : DomainEvent
    {
        public string UserId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Action { get; set; } = string.Empty; // "Add", "Remove", "Update"
        public DateTime UpdatedAt { get; set; }
        
        public override string Data => System.Text.Json.JsonSerializer.Serialize(this);
    }

    public class OrderShippedEvent : DomainEvent
    {
        public Guid OrderId { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public string ShippingCompany { get; set; } = string.Empty;
        public DateTime ShippedAt { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        
        public override string Data => System.Text.Json.JsonSerializer.Serialize(this);
    }

    public class OrderDeliveredEvent : DomainEvent
    {
        public Guid OrderId { get; set; }
        public DateTime DeliveredAt { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public string DeliveryNotes { get; set; } = string.Empty;
        
        public override string Data => System.Text.Json.JsonSerializer.Serialize(this);
    }

    public class RefundProcessedEvent : DomainEvent
    {
        public Guid OrderId { get; set; }
        public string RefundId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        
        public override string Data => System.Text.Json.JsonSerializer.Serialize(this);
    }

    public class LowStockAlertEvent : DomainEvent
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int Threshold { get; set; }
        public DateTime AlertedAt { get; set; }
        
        public override string Data => System.Text.Json.JsonSerializer.Serialize(this);
    }
}
