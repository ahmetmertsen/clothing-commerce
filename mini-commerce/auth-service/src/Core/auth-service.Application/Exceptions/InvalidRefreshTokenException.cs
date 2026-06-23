using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Exceptions
{
    public class InvalidRefreshTokenException : ApplicationException
    {
        public InvalidRefreshTokenException(string message)
            : base(message, 401, "INVALID_REFRESH_TOKEN")
        {
        }
    }
}
