using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Dtos.User
{
    public class UpdateUserMailVerifyRequest
    {
        public Guid UserId { get; set; }
        public string EmailConfirmToken { get; set; }
    }
}
