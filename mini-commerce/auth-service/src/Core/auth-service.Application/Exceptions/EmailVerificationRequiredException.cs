using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Exceptions
{
    public class EmailVerificationRequiredException : ApplicationException
    {
        public EmailVerificationRequiredException(string message)
            : base(message, 403, "EMAIL_VERIFICATION_REQUIRED")
        {
        }
    }
}
