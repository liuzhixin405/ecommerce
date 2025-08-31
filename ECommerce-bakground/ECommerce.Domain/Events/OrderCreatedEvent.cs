using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Events
{
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemInfo> Items { get; set; } = new List<OrderItemInfo>();

        public OrderCreatedEvent(Order order)
        {
            OrderId = order.Id;
            UserId = order.UserId;
            TotalAmount = order.TotalAmount;
            CreatedAt = order.CreatedAt;
            Items = order.Items.Select(item => new OrderItemInfo
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Price
            }).ToList();
        }
    }

    public class OrderItemInfo
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}