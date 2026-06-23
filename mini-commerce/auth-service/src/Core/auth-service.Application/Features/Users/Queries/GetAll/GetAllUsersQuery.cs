using auth_service.Application.Dtos;
using auth_service.Application.Dtos.User;
using auth_service.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Users.Queries.GetAll
{
    public class GetAllUsersQuery : IRequest<PagedResponse<AdminUserDto>>
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 20;
        public string? Search { get; set; }
        public UserStatus? Status { get; set; }
        public bool? EmailConfirmed { get; set; }
    }
}
