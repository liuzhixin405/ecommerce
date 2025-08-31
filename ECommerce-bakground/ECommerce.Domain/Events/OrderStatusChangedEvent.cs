using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Events
{
    public class OrderStatusChangedEvent
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public OrderStatus OldStatus { get; set; }
        public OrderStatus NewStatus { get; set; }
        public DateTime ChangedAt { get; set; }

        public OrderStatusChangedEvent(Guid orderId, Guid userId, OrderStatus oldStatus, OrderStatus newStatus)
        {
            OrderId = orderId;
            UserId = userId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            ChangedAt = DateTime.UtcNow;
        }
    }
}
