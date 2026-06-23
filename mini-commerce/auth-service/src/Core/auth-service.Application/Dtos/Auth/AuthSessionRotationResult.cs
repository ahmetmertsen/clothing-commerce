using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Dtos.Auth
{
    public class AuthSessionRotationResult
    {
        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }
    }
}
