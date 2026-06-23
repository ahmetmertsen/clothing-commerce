using auth_service.Application.Features.Auth.Register;
using auth_service.Application.Features.Users.Commands.AssignRoleToUser;
using auth_service.Application.Features.Users.Commands.Update.UpdateEmail;
using auth_service.Application.Features.Users.Commands.Update.UpdateMailVerify;
using auth_service.Application.Features.Users.Commands.Update.UpdatePassword;
using auth_service.Application.Features.Users.Queries.GetAll;
using auth_service.Application.Features.Users.Queries.GetById;
using auth_service.Application.Features.Users.Queries.GetRolesToUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace auth_service.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediatR;

        public UserController(IMediator mediatR)
        {
            _mediatR = mediatR;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [HttpPost]
        [Route("updatePassword")]
        public async Task<IActionResult> UpdateUserPassword([FromBody] UpdateUserPasswordCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("updateMailVerify")]
        public async Task<IActionResult> UpdateUserMailVerify([FromBody] UpdateUserMailVerifyCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }


        [Authorize]
        [HttpPost]
        [Route("updateUserEmail")]
        public async Task<IActionResult> UpdateUserEmail([FromBody] UpdateUserEmailCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("getAllUsers")]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUsersQuery request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        [Route("getUserById/{userId:guid}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var response = await _mediatR.Send(new GetUserByIdQuery() { UserId = userId });
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("getRolesToUser/{userId:guid}")]
        public async Task<IActionResult> GetRolesToUser(Guid userId)
        {
            var response = await _mediatR.Send(new GetRolesToUserQuery() { UserId = userId });
            return Ok(response);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("assignRoleToUser")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleToUserCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }
    }
}
