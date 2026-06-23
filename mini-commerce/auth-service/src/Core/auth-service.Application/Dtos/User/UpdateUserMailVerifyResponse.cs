using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Dtos.User
{
    public class UpdateUserMailVerifyResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
    }
}
