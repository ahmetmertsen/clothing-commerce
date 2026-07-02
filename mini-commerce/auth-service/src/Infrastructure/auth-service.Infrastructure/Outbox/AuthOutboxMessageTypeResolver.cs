using Shared.Events.AuthEvents;

namespace auth_service.Infrastructure.Outbox
{
    internal static class AuthOutboxMessageTypeResolver
    {
        private static readonly IReadOnlyDictionary<string, Type> MessageTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(NotificationRequested)] = typeof(NotificationRequested),
            [typeof(NotificationRequested).FullName!] = typeof(NotificationRequested),
            [nameof(AuthUserRegisteredEvent)] = typeof(AuthUserRegisteredEvent),
            [typeof(AuthUserRegisteredEvent).FullName!] = typeof(AuthUserRegisteredEvent),
            [nameof(AuthUserEmailChangedEvent)] = typeof(AuthUserEmailChangedEvent),
            [typeof(AuthUserEmailChangedEvent).FullName!] = typeof(AuthUserEmailChangedEvent)
        };

        public static Type Resolve(string type)
        {
            if (MessageTypes.TryGetValue(type, out var messageType))
            {
                return messageType;
            }

            throw new InvalidOperationException($"Unsupported auth outbox message type: {type}");
        }
    }
}
