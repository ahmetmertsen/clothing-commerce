using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Exceptions
{
    public class ChangeEmailFailedException : ApplicationException
    {
        public ChangeEmailFailedException(string message)
        : base(message, 400, "EMAIL_CHANGE_FAILED")
        {
        }
    }
}
