using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Dtos.User
{
    public class UpdateUserPasswordRequest
    {
        public Guid UserId { get; set; }
        public string ResetToken { get; set; }
        public string newPassword { get; set; }
        public string newPasswordConfirmed { get; set; }
    }
}
