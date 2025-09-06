namespace ECommerce.Domain.Interfaces
{
    public interface IOrderMessagePublisher
    {
        Task PublishShipmentMessageAsync(Guid orderId, Guid userId);
        Task PublishCompletionMessageAsync(Guid orderId, Guid userId);
    }
}
