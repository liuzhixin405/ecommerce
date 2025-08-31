using ECommerce.Core.EventBus;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.EventBus
{
    public class InMemoryEventBus : IEventBus
    {
        private readonly Dictionary<Type, List<object>> _handlers = new();
        private readonly ILogger<InMemoryEventBus> _logger;

        public InMemoryEventBus(ILogger<InMemoryEventBus> logger)
        {
            _logger = logger;
        }

        public async Task PublishAsync<T>(T @event) where T : class
        {
            var eventType = typeof(T);
            
            if (_handlers.ContainsKey(eventType))
            {
                var handlers = _handlers[eventType];
                var tasks = new List<Task>();

                foreach (var handler in handlers)
                {
                    if (handler is IEventHandler<T> eventHandler)
                    {
                        tasks.Add(eventHandler.HandleAsync(@event));
                    }
                }

                await Task.WhenAll(tasks);
                _logger.LogInformation("Published event {EventType} to {HandlerCount} handlers", 
                    eventType.Name, handlers.Count);
            }
            else
            {
                _logger.LogWarning("No handlers found for event type {EventType}", eventType.Name);
            }
        }

        public void Subscribe<T>(IEventHandler<T> handler) where T : class
        {
            var eventType = typeof(T);
            
            if (!_handlers.ContainsKey(eventType))
            {
                _handlers[eventType] = new List<object>();
            }
            
            _handlers[eventType].Add(handler);
            _logger.LogInformation("Subscribed handler {HandlerType} to event {EventType}", 
                handler.GetType().Name, eventType.Name);
        }
    }
}
