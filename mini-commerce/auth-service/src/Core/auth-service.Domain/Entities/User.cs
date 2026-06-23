using auth_service.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public DateTime? EmailVerificationSentAt { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public Enums.UserStatus Status { get; set; } = Enums.UserStatus.Active;
        public DateTime? SuspendedUntil { get; set; }
        public ICollection<AuthSession> AuthSessions { get; set; } = new List<AuthSession>();
    }
}
