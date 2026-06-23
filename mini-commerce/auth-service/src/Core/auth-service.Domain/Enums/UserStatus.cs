using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Domain.Enums
{
    public enum UserStatus
    {
        Active = 1,
        Suspended = 2,
        Banned = 3
    }
}
