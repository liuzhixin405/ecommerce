using ECommerce.Domain.Models;

namespace ECommerce.Domain.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(Guid userId);
        Task<OrderDto?> GetOrderByIdAsync(Guid id);
        Task<OrderDto> CreateOrderAsync(Guid userId, CreateOrderDto createOrderDto);
        Task<OrderDto> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusDto updateOrderStatusDto);
        Task<bool> CancelOrderAsync(Guid id);
        Task<bool> ProcessPaymentAsync(PaymentDto paymentDto);
        Task<bool> ShipOrderAsync(Guid id, string trackingNumber);
        Task<bool> DeliverOrderAsync(Guid id);
        Task<bool> CompleteOrderAsync(Guid id);
        Task<bool> CancelExpiredOrdersAsync();
    }
}
