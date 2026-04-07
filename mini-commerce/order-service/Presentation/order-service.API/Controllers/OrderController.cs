using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using order_service.Application.Features.Order.Commands.Create;
using order_service.Application.Features.Order.Queries.GetOrderById;
using order_service.Application.Features.Order.Queries.GetOrderByOrderNumber;
using order_service.Application.Features.Order.Queries.GetOrdersByCustomerId;

namespace order_service.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediatR;

        public OrderController(IMediator mediator)
        {
            _mediatR = mediator;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [HttpGet]
        [Route("getOrderById/{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var response = await _mediatR.Send(new GetOrderByIdRequest { Id = id });
            return Ok(response);
        }

        [HttpGet]
        [Route("getOrderByOrderNumber/{orderNumber}")]
        public async Task<IActionResult> GetOrderByOrderNumber(string orderNumber)
        {
            var response = await _mediatR.Send(new GetOrderByOrderNumberRequest { OrderNumber = orderNumber });
            return Ok(response);
        }

        [HttpGet]
        [Route("getOrdersByCustomerId/{customerId}")]
        public async Task<IActionResult> GetOrdersByCustomerId(Guid customerId)
        {
            var response = await _mediatR.Send(new GetOrdersByCustomerIdRequest { CustomerId = customerId });
            return Ok(response);
        }
    }
}
