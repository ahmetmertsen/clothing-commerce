namespace Shared.Events.AuthEvents
{
    public class AuthUserEmailChangedEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public Guid AuthUserId { get; set; }
        public string OldEmail { get; set; } = string.Empty;
        public string NewEmail { get; set; } = string.Empty;
        public Guid CorrelationId { get; set; }
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}
