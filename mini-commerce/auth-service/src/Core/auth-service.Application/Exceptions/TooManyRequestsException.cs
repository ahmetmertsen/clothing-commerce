using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Exceptions
{
    public class TooManyRequestsException : ApplicationException
    {
        public TooManyRequestsException(string message) : base(message, 429, "TOO_MANY_REQUESTS")
        {
        }
    }
}
