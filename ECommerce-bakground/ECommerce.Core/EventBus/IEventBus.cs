namespace ECommerce.Core.EventBus
{
    public interface IEventBus
    {
        Task PublishAsync<T>(T @event) where T : class;
        void Subscribe<T>(IEventHandler<T> handler) where T : class;
    }

    public interface IEventHandler<in T> where T : class
    {
        Task HandleAsync(T @event);
    }
}
