using ECommerce.Domain.Models;

namespace ECommerce.Domain.Interfaces
{
    /// <summary>
    /// 事件处理器通用接口
    /// </summary>
    /// <typeparam name="TEvent">要处理的事件类型</typeparam>
    public interface IEventHandler<in TEvent> where TEvent : DomainEvent
    {
        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="domainEvent">领域事件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>处理结果</returns>
        Task<bool> HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 事件处理器工厂接口
    /// </summary>
    public interface IEventHandlerFactory
    {
        /// <summary>
        /// 获取指定事件类型的所有处理器
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>事件处理器集合</returns>
        IEnumerable<object> GetHandlers(string eventType);

        /// <summary>
        /// 注册事件处理器
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <typeparam name="THandler">处理器类型</typeparam>
        void RegisterHandler<TEvent, THandler>() where TEvent : DomainEvent where THandler : IEventHandler<TEvent>;
    }
}
