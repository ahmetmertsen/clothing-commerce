namespace auth_service.API.Configurations.RateLimiting
{
    public class SensitiveEndpointRateLimitOptions
    {
        public Dictionary<string, FixedWindowRateLimitPolicy> Policies { get; set; } = new();
    }

    public class FixedWindowRateLimitPolicy
    {
        public int PermitLimit { get; set; }
        public int WindowSeconds { get; set; }
    }
}
