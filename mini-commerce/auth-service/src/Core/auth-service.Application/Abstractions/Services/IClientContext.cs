using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Abstractions.Services
{
    public interface IClientContext
    {
        string? DeviceName { get; }
        string? UserAgent { get; }
        string? IpAddress { get; }
    }
}
