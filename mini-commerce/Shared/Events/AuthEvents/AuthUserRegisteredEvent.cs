namespace Shared.Events.AuthEvents
{
    public class AuthUserRegisteredEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public Guid AuthUserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid CorrelationId { get; set; }
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}
