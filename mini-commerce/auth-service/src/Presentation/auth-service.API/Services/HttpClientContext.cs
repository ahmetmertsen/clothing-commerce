using auth_service.Application.Abstractions.Services;

namespace auth_service.API.Services
{
    public class HttpClientContext : IClientContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpClientContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? DeviceName => GetHeader("X-Device-Name");
        public string? UserAgent => GetHeader("User-Agent");
        public string? IpAddress => _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        private string? GetHeader(string name)
        {
            var value = _httpContextAccessor.HttpContext?.Request.Headers[name].ToString();
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
    }
}
