using auth_service.Application.Features.Auth.ChangeEmail;
using auth_service.Application.Features.Auth.ForgotPassword;
using auth_service.Application.Features.Auth.GetSessions;
using auth_service.Application.Features.Auth.Login;
using auth_service.Application.Features.Auth.Logout;
using auth_service.Application.Features.Auth.LogoutAll;
using auth_service.Application.Features.Auth.MailVerify;
using auth_service.Application.Features.Auth.RefreshTokenLogin;
using auth_service.Application.Features.Auth.RevokeSession;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace auth_service.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediatR;
        public AuthController(IMediator mediatR)
        {
            _mediatR = mediatR;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [HttpPost]
        [Route("refreshTokenLogin")]
        public async Task<IActionResult> RefreshTokenLogin([FromBody] RefreshTokenLoginCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [HttpPost]
        [Route("forgotPassword")]
        public async Task<IActionResult> ForgotPasswordReset([FromBody] ForgotPasswordCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("mailVerify")]
        public async Task<IActionResult> MailVerify()
        {
            var response = await _mediatR.Send(new MailVerifyCommand());
            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("emailChange")]
        public async Task<IActionResult> EmailChange([FromBody] ChangeEmailCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            var response = await _mediatR.Send(new LogoutCommand());
            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("logoutAll")]
        public async Task<IActionResult> LogoutAll()
        {
            var response = await _mediatR.Send(new LogoutAllCommand());
            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        [Route("sessions")]
        public async Task<IActionResult> GetSessions()
        {
            var response = await _mediatR.Send(new GetAuthSessionsQuery());
            return Ok(response);
        }

        [Authorize]
        [HttpDelete]
        [Route("sessions/{sessionId:guid}")]
        public async Task<IActionResult> RevokeSession(Guid sessionId)
        {
            var response = await _mediatR.Send(new RevokeSessionCommand { SessionId = sessionId });
            return Ok(response);
        }
    }
}
