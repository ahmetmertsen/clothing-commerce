using auth_service.API.Configurations.RateLimiting;
using auth_service.API.Models;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;

namespace auth_service.API.Middlewares
{
    public class SensitiveEndpointRateLimitMiddleware
    {
        private static readonly ConcurrentDictionary<string, FixedWindowCounter> Counters = new();
        private static int _requestCount;

        private readonly RequestDelegate _next;
        private readonly SensitiveEndpointRateLimitOptions _options;
        private readonly ILogger<SensitiveEndpointRateLimitMiddleware> _logger;

        public SensitiveEndpointRateLimitMiddleware(
            RequestDelegate next,
            IOptions<SensitiveEndpointRateLimitOptions> options,
            ILogger<SensitiveEndpointRateLimitMiddleware> logger)
        {
            _next = next;
            _options = options.Value;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var policyEntry = _options.Policies.FirstOrDefault(entry =>
                string.Equals(entry.Key, context.Request.Path.Value, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrWhiteSpace(policyEntry.Key) ||
                policyEntry.Value.PermitLimit <= 0 ||
                policyEntry.Value.WindowSeconds <= 0)
            {
                await _next(context);
                return;
            }

            var now = DateTimeOffset.UtcNow;
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var counterKey = $"{policyEntry.Key}:{clientIp}";
            var counter = Counters.GetOrAdd(
                counterKey,
                _ => new FixedWindowCounter(now.AddSeconds(policyEntry.Value.WindowSeconds)));

            int retryAfterSeconds;
            lock (counter.SyncRoot)
            {
                if (counter.WindowEndsAt <= now)
                {
                    counter.Count = 0;
                    counter.WindowEndsAt = now.AddSeconds(policyEntry.Value.WindowSeconds);
                }

                if (counter.Count >= policyEntry.Value.PermitLimit)
                {
                    retryAfterSeconds = Math.Max(1, (int)Math.Ceiling((counter.WindowEndsAt - now).TotalSeconds));
                }
                else
                {
                    counter.Count++;
                    retryAfterSeconds = 0;
                }
            }

            CleanupExpiredCounters(now);

            if (retryAfterSeconds == 0)
            {
                await _next(context);
                return;
            }

            _logger.LogWarning(
                "Sensitive endpoint rate limit exceeded. Path: {Path}, ClientIp: {ClientIp}, RetryAfterSeconds: {RetryAfterSeconds}",
                context.Request.Path,
                clientIp,
                retryAfterSeconds);

            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
            var response = new ApiResponse
            {
                IsSuccess = false,
                Error = new ErrorResponse
                {
                    Code = "RATE_LIMIT_EXCEEDED",
                    Message = "Çok fazla istek gönderildi. Lütfen daha sonra tekrar deneyin.",
                    HttpStatus = StatusCodes.Status429TooManyRequests,
                    TraceId = traceId
                }
            };

            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = "application/json";
            context.Response.Headers.RetryAfter = retryAfterSeconds.ToString();
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }

        private static void CleanupExpiredCounters(DateTimeOffset now)
        {
            if (Interlocked.Increment(ref _requestCount) % 500 != 0)
            {
                return;
            }

            foreach (var entry in Counters)
            {
                if (entry.Value.WindowEndsAt <= now)
                {
                    Counters.TryRemove(entry.Key, out _);
                }
            }
        }

        private sealed class FixedWindowCounter
        {
            public FixedWindowCounter(DateTimeOffset windowEndsAt)
            {
                WindowEndsAt = windowEndsAt;
            }

            public object SyncRoot { get; } = new();
            public int Count { get; set; }
            public DateTimeOffset WindowEndsAt { get; set; }
        }
    }
}
