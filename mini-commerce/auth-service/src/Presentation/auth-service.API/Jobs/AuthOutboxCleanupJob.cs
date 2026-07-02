using auth_service.API.Configurations.Outbox;
using auth_service.Application.Repositories;
using Microsoft.Extensions.Options;
using Quartz;

namespace auth_service.API.Jobs
{
    [DisallowConcurrentExecution]
    public class AuthOutboxCleanupJob : IJob
    {
        private readonly IAuthOutboxRepository _outboxRepository;
        private readonly IOptions<AuthOutboxOptions> _options;
        private readonly ILogger<AuthOutboxCleanupJob> _logger;

        public AuthOutboxCleanupJob(IAuthOutboxRepository outboxRepository, IOptions<AuthOutboxOptions> options, ILogger<AuthOutboxCleanupJob> logger)
        {
            _outboxRepository = outboxRepository;
            _options = options;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var cancellationToken = context.CancellationToken;
            var options = _options.Value;
            var now = DateTime.UtcNow;

            var processedOlderThan = now.AddMinutes(-Math.Max(1, options.ProcessedRetentionMinutes));
            var sensitiveOlderThan = now.AddMinutes(-Math.Max(1, options.SensitiveRetentionMinutes));

            var processedDeleted = await _outboxRepository.CleanupProcessedAsync(processedOlderThan, cancellationToken);
            var sensitiveDeleted = await _outboxRepository.CleanupExpiredSensitiveAsync(sensitiveOlderThan, cancellationToken);

            if (processedDeleted > 0 || sensitiveDeleted > 0)
            {
                _logger.LogInformation(
                    "Auth outbox cleanup completed. ProcessedDeleted: {ProcessedDeleted}, SensitiveDeleted: {SensitiveDeleted}",
                    processedDeleted,
                    sensitiveDeleted);
            }
        }
    }
}
