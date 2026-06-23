using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Dtos.User
{
    public class UpdateUserEmailRequest
    {
        public Guid UserId { get; set; }
        public string ChangeEmailToken { get; set; }
        public string NewEmail { get; set; }
    }
}
