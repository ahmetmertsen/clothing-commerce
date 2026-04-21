using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using shipment_service.Application.Features.Shipment.Commands.Create;
using shipment_service.Application.Features.Shipment.Queries.GetById;
using shipment_service.Application.Features.Shipment.Queries.GetByOrderId;

namespace shipment_service.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentController : ControllerBase
    {
        private readonly IMediator _mediatR;

        public ShipmentController(IMediator mediatR)
        {
            _mediatR = mediatR;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] CreateShipmentCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [HttpGet]
        [Route("getById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _mediatR.Send(new GetShipmentByIdRequest { Id = id });
            return Ok(response);
        }

        [HttpGet]
        [Route("getByOrderId/{orderId}")]
        public async Task<IActionResult> GetByOrderId(Guid orderId)
        {
            var response = await _mediatR.Send(new GetShipmentsByOrderIdRequest { OrderId = orderId });
            return Ok(response);
        }
    }
}
