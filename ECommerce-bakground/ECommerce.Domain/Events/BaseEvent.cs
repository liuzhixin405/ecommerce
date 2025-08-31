namespace ECommerce.Domain.Events
{
    public abstract class BaseEvent
    {
        public Guid EventId { get; set; }
        public DateTime OccurredOn { get; set; }
        public string EventType { get; set; }
        
        protected BaseEvent()
        {
            EventId = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            EventType = GetType().Name;
        }
    }
}