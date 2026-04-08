using cart_service.Application.Features.Cart.Commands.AddItemToCart;
using cart_service.Application.Features.Cart.Commands.ChangeCartItemQuantity;
using cart_service.Application.Features.Cart.Commands.ClearCart;
using cart_service.Application.Features.Cart.Commands.RemoveItemFromCart;
using cart_service.Application.Features.Cart.Queries.GetCartByCustomerId;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cart_service.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediatR;

        public CartController(IMediator mediatR)
        {
            _mediatR = mediatR;
        }

        [HttpPost]
        [Route("addItemToCart")]
        public async Task<IActionResult> AddItemToCart([FromBody] AddItemToCartCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [HttpPut]
        [Route("changeCartItemQuantity")]
        public async Task<IActionResult> ChangeCartItemQuantity([FromBody] ChangeCartItemQuantityCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [HttpDelete]
        [Route("clearCart/{customerId}")]
        public async Task<IActionResult> ClearCart(Guid customerId)
        {
            var response = await _mediatR.Send(new ClearCartCommand { CustomerId = customerId });
            return Ok(response);
        }

        [HttpDelete]
        [Route("removeItemFromCart")]
        public async Task<IActionResult> RemoveItemFromCart([FromBody] RemoveItemFromCartCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [HttpGet]
        [Route("getCartByCustomerId/{customerId}")]
        public async Task<IActionResult> GetCartByCustomerId(Guid customerId)
        {
            var response = await _mediatR.Send(new GetCartByCustomerIdRequest { CustomerId = customerId });
            return Ok(response);
        }

    }
}
