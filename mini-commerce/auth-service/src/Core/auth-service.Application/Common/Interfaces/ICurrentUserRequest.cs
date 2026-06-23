using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Common.Interfaces
{
    public interface ICurrentUserRequest
    {
        public Guid UserId { get; set; }
    }
}
