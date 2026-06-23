using auth_service.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Dtos.User
{
    public class AdminUserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public UserStatus Status { get; set; }
        public DateTime? SuspendedUntil { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool IsLockedOut { get; set; }
        public string[] Roles { get; set; } = Array.Empty<string>();
    }
}
