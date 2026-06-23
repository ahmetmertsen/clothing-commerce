using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Dtos.User
{
    public class UpdateUserEmailResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public Guid UserId { get; set; }
    }
}
