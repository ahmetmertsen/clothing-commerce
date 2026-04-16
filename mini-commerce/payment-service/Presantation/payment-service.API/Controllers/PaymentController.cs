using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using payment_service.Application.Features.Payment.Commands.ProcessPayment;
using payment_service.Application.Features.Payment.Queries.GetPaymentById;
using payment_service.Application.Features.Payment.Queries.GetPaymentsByCustomerId;

namespace payment_service.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediatR;

        public PaymentController(IMediator mediatR)
        {
            _mediatR = mediatR;
        }

        [HttpPost]
        [Route("processPayment")]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [HttpGet]
        [Route("getById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _mediatR.Send(new GetPaymentByIdRequest { Id = id });
            return Ok(response);
        }

        [HttpGet]
        [Route("getByCustomerId/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(Guid customerId)
        {
            var response = await _mediatR.Send(new GetPaymentsByCustomerIdRequest { CustomerId = customerId });
            return Ok(response);
        }
    }
}
