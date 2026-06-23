using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Users.Queries.GetRolesToUser
{
    public class GetRolesToUserQuery : IRequest<GetRolesToUserQueryResponse>
    {
        public Guid UserId { get; set; }
    }
}
