using auth_service.Application.Features.Roles.Commands.Create;
using auth_service.Application.Features.Roles.Commands.Delete;
using auth_service.Application.Features.Roles.Commands.Update;
using auth_service.Application.Features.Roles.Queries.GetAll;
using auth_service.Application.Features.Roles.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace auth_service.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediatR;

        public RoleController(IMediator mediatR)
        {
            _mediatR = mediatR;
        }

        [HttpGet]
        [Route("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _mediatR.Send(new GetAllRolesQuery());
            return Ok(response);
        }

        [HttpGet]
        [Route("getById/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _mediatR.Send(new GetRoleByIdQuery { Id = id });
            return Ok(response);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] CreateRoleCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UpdateRoleCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [HttpDelete]
        [Route("delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _mediatR.Send(new DeleteRoleCommand { Id = id });
            return Ok(response);
        }
    }
}
