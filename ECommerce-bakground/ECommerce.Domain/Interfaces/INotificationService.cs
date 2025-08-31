using ECommerce.Domain.Models;

namespace ECommerce.Domain.Interfaces
{
    public interface INotificationService
    {
        Task SendLowStockNotificationAsync(InventoryUpdatedEvent inventoryEvent);
        Task SendOrderStatusNotificationAsync(string orderId, string customerId, string status);
        Task SendPaymentNotificationAsync(PaymentProcessedEvent paymentEvent);
        Task SendInventoryAlertAsync(StockLockedEvent stockEvent);
        Task SendSystemAlertAsync(string message, string level);
        Task SendCustomerNotificationAsync(string customerId, string message, string type);
    }
}
