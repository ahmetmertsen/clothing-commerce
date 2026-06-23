using auth_service.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Users.Queries.GetRolesToUser
{
    public class GetRolesToUserQueryHandler : IRequestHandler<GetRolesToUserQuery, GetRolesToUserQueryResponse>
    {
        private readonly IUserService _userService;

        public GetRolesToUserQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<GetRolesToUserQueryResponse> Handle(GetRolesToUserQuery request, CancellationToken cancellationToken)
        {
            var roles = await _userService.GetRolesToUserAsync(request.UserId);
            GetRolesToUserQueryResponse response = new()
            {
                UserId = request.UserId,
                Roles = roles
            };
            return response;
        }
    }
}
