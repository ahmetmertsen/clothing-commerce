using auth_service.Application.Abstractions.Services;
using auth_service.Application.Dtos.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth_service.Application.Features.Users.Commands.Update.UpdateEmail
{
    public class UpdateUserEmailCommandHandler : IRequestHandler<UpdateUserEmailCommand, UpdateUserEmailCommandResponse>
    {
        private readonly IUserService _userService;

        public UpdateUserEmailCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UpdateUserEmailCommandResponse> Handle(UpdateUserEmailCommand commandRequest, CancellationToken cancellationToken)
        {
            UpdateUserEmailRequest request = new()
            {
                UserId = commandRequest.UserId,
                ChangeEmailToken = commandRequest.ChangeEmailToken,
                NewEmail = commandRequest.NewEmail,
            };

            var response = await _userService.UpdateUserEmailAsync(request);

            UpdateUserEmailCommandResponse commandResponse = new(Succeeded: response.Succeeded, Message: response.Message);
            return commandResponse;
        }
    }
}
