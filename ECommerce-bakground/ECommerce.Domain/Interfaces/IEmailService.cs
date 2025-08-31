using ECommerce.Domain.Models;

namespace ECommerce.Domain.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendOrderConfirmationAsync(OrderCreatedEvent orderEvent);
        Task<bool> SendPaymentConfirmationAsync(OrderPaidEvent orderEvent);
        Task<bool> SendOrderCancellationAsync(OrderCancelledEvent orderEvent);
        Task<bool> SendLowStockAlertAsync(InventoryUpdatedEvent inventoryEvent);
        Task<bool> SendRefundConfirmationAsync(string orderId, string customerEmail, decimal amount);
        Task<bool> SendOrderShippedAsync(string orderId, string customerEmail, string trackingNumber);
        Task<bool> SendWelcomeEmailAsync(string customerEmail, string customerName);
    }
}
