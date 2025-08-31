using ECommerce.Application.EventHandlers;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Services
{
    /// <summary>
    /// 事件处理器工厂实现
    /// </summary>
    public class EventHandlerFactory : IEventHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventHandlerFactory> _logger;
        private readonly Dictionary<string, Type> _handlerTypes;

        public EventHandlerFactory(IServiceProvider serviceProvider, ILogger<EventHandlerFactory> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _handlerTypes = new Dictionary<string, Type>();

            // 注册所有事件处理器
            RegisterAllHandlers();
        }

        public IEnumerable<object> GetHandlers(string eventType)
        {
            try
            {
                if (_handlerTypes.TryGetValue(eventType, out var handlerType))
                {
                    var handler = _serviceProvider.GetService(handlerType);
                    if (handler != null)
                    {
                        return new[] { handler };
                    }
                }

                _logger.LogWarning("No handlers found for event type: {EventType}", eventType);
                return Enumerable.Empty<object>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting handlers for event type: {EventType}", eventType);
                return Enumerable.Empty<object>();
            }
        }

        public void RegisterHandler<TEvent, THandler>() where TEvent : DomainEvent where THandler : IEventHandler<TEvent>
        {
            var eventType = typeof(TEvent).Name;
            var handlerType = typeof(THandler);

            if (_handlerTypes.ContainsKey(eventType))
            {
                _logger.LogWarning("Handler for event type {EventType} is already registered. Overwriting with {HandlerType}", 
                    eventType, handlerType.Name);
            }

            _handlerTypes[eventType] = handlerType;
            _logger.LogInformation("Registered handler {HandlerType} for event type {EventType}", 
                handlerType.Name, eventType);
        }

        private void RegisterAllHandlers()
        {
            // 注册所有事件处理器
            RegisterHandler<OrderCreatedEvent, OrderCreatedEventHandler>();
            RegisterHandler<OrderPaidEvent, OrderStatusChangedEventHandler>();
            RegisterHandler<OrderCancelledEvent, OrderCancelledEventHandler>();
            RegisterHandler<InventoryUpdatedEvent, InventoryUpdatedEventHandler>();
            RegisterHandler<PaymentProcessedEvent, PaymentProcessedEventHandler>();
            RegisterHandler<UserRegisteredEvent, UserRegisteredEventHandler>();

            _logger.LogInformation("Registered {Count} event handlers", _handlerTypes.Count);
        }
    }
}
