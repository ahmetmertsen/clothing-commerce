using auth_service.Application.Abstractions.Services;
using auth_service.Application.Dtos.User;
using AutoMapper;
using MediatR;

namespace auth_service.Application.Features.Auth.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserCommandResponse>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public RegisterUserCommandHandler(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<RegisterUserCommandResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            RegisterUserRequestDto userDto = _mapper.Map<RegisterUserRequestDto>(request);
            RegisterUserResponseDto response = await _userService.RegisterAsync(userDto, cancellationToken);
            if (response.Succeeded)
            {
                return new RegisterUserCommandResponse(Succeeded: true, Message: response.Message);
            }
            else
            {
                return new RegisterUserCommandResponse(Succeeded: response.Succeeded, Message: response.Message);
            }
        }
    }
}
