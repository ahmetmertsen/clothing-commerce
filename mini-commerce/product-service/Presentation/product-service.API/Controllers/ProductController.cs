using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using product_service.Application.Features.Product.Commands.AddVariant;
using product_service.Application.Features.Product.Commands.Create;
using product_service.Application.Features.Product.Commands.Update;
using product_service.Application.Features.Product.Commands.UpdateVariantStock;
using product_service.Application.Features.Product.Queries.GetAll;
using product_service.Application.Features.Product.Queries.GetById;
using product_service.Application.Features.Product.Queries.GetByVariantId;

namespace product_service.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediatR;

        public ProductController(IMediator mediatR)
        {
            _mediatR = mediatR;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UpdateProductCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [HttpPost]
        [Route("addVariant")]
        public async Task<IActionResult> AddProductVariant([FromBody] AddProductVariantCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [HttpPut]
        [Route("updateVariantStock")]
        public async Task<IActionResult> UpdateProductVariantStock([FromBody] UpdateProductVariantStockCommand request)
        {
            var response = await _mediatR.Send(request);
            return Ok(response);
        }

        [HttpGet]
        [Route("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _mediatR.Send(new GetAllProductsRequest());
            return Ok(response);
        }

        [HttpGet]
        [Route("getById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _mediatR.Send(new GetProductByIdRequest { Id = id });
            return Ok(response);
        }

        [HttpGet]
        [Route("getByVariantId/{variantId}")]
        public async Task<IActionResult> GetByVariantId(Guid variantId)
        {
            var response = await _mediatR.Send(new GetProductByVariantIdRequest { VariantId = variantId });
            return Ok(response);
        }
    }
}
