using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Dtos.Auth
{
    public class AuthTokenDto
    {
        public string AccessToken { get; set; }
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; }
        public Guid SessionId { get; set; }
        public bool RequiresEmailVerification { get; set; }
    }
}
