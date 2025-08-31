using ECommerce.Domain.Models;

namespace ECommerce.Domain.Interfaces
{
    public interface IEventPublisher
    {
        /// <summary>
        /// 发布单个事件
        /// </summary>
        /// <param name="domainEvent">领域事件</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task PublishAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量发布事件
        /// </summary>
        /// <param name="domainEvents">领域事件集合</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task PublishAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken = default);

        /// <summary>
        /// 发布Outbox消息
        /// </summary>
        /// <param name="outboxMessage">Outbox消息</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task PublishOutboxMessageAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken = default);
    }
}