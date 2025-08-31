using ECommerce.Domain.Events;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.MessageQueue;

namespace ECommerce.Infrastructure.EventBus
{
    public class RabbitMQEventPublisher : IEventPublisher
    {
        private readonly RabbitMQService _rabbitMQService;

        public RabbitMQEventPublisher(RabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        public async Task PublishAsync<T>(T @event) where T : BaseEvent
        {
            // 在实际应用中，我们可能需要根据事件类型使用不同的队列
            var queueName = @event.EventType;
            _rabbitMQService.Publish(queueName, @event);
            
            // 这里为了简化使用了同步方法，实际应用中可能需要异步处理
            await Task.CompletedTask;
        }
    }
}