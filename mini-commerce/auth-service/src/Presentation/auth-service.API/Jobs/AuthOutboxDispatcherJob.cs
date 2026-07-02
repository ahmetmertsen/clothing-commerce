using auth_service.API.Configurations.Outbox;
using auth_service.Application.Abstractions.Services;
using auth_service.Application.Repositories;
using auth_service.Domain.Entities;
using Microsoft.Extensions.Options;
using Quartz;

namespace auth_service.API.Jobs
{
    [DisallowConcurrentExecution]
    public class AuthOutboxDispatcherJob : IJob
    {
        private readonly IAuthOutboxRepository _outboxRepository;
        private readonly IAuthOutboxMessagePublisher _messagePublisher;
        private readonly IOptions<AuthOutboxOptions> _options;
        private readonly ILogger<AuthOutboxDispatcherJob> _logger;

        public AuthOutboxDispatcherJob(IAuthOutboxRepository outboxRepository, IAuthOutboxMessagePublisher messagePublisher, IOptions<AuthOutboxOptions> options, ILogger<AuthOutboxDispatcherJob> logger)
        {
            _outboxRepository = outboxRepository;
            _messagePublisher = messagePublisher;
            _options = options;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var cancellationToken = context.CancellationToken;
            var options = _options.Value;
            var lockedBy = $"{Environment.MachineName}:{context.FireInstanceId}";
            var messages = await _outboxRepository.ClaimPendingAsync(
                Math.Max(1, options.BatchSize),
                TimeSpan.FromSeconds(Math.Max(10, options.LockSeconds)),
                lockedBy,
                cancellationToken);

            foreach (var message in messages)
            {
                await DispatchAsync(message, options, cancellationToken);
            }
        }

        private async Task DispatchAsync(AuthOutbox message, AuthOutboxOptions options, CancellationToken cancellationToken)
        {
            try
            {
                await _messagePublisher.PublishAsync(message, cancellationToken);
                await _outboxRepository.MarkProcessedAsync(message.IdempotentToken, DateTime.UtcNow, cancellationToken);

                _logger.LogInformation(
                    "Auth outbox message dispatched. IdempotentToken: {IdempotentToken}, CorrelationId: {CorrelationId}, Type: {Type}",
                    message.IdempotentToken,
                    message.CorrelationId,
                    message.Type);
            }
            catch (Exception exception)
            {
                var nextAttemptAt = CalculateNextAttemptAt(message, options);
                await _outboxRepository.MarkFailedAsync(message.IdempotentToken, exception.Message, nextAttemptAt, cancellationToken);

                _logger.LogWarning(
                    exception,
                    "Auth outbox message dispatch failed. IdempotentToken: {IdempotentToken}, CorrelationId: {CorrelationId}, Type: {Type}, RetryCount: {RetryCount}, NextAttemptAt: {NextAttemptAt}",
                    message.IdempotentToken,
                    message.CorrelationId,
                    message.Type,
                    message.RetryCount + 1,
                    nextAttemptAt);
            }
        }

        private static DateTime? CalculateNextAttemptAt(AuthOutbox message, AuthOutboxOptions options)
        {
            var nextRetryCount = message.RetryCount + 1;
            if (nextRetryCount >= message.MaxRetryCount)
            {
                return null;
            }

            var retryBaseDelaySeconds = Math.Max(1, options.RetryBaseDelaySeconds);
            var maxRetryDelaySeconds = Math.Max(retryBaseDelaySeconds, options.MaxRetryDelaySeconds);
            var delaySeconds = Math.Min(maxRetryDelaySeconds, retryBaseDelaySeconds * Math.Pow(2, Math.Max(0, nextRetryCount - 1)));

            return DateTime.UtcNow.AddSeconds(delaySeconds);
        }
    }
}
