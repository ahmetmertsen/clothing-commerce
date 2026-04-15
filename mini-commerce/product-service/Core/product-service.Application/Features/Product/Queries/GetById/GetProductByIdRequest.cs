using MediatR;
using product_service.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace product_service.Application.Features.Product.Queries.GetById
{
    public class GetProductByIdRequest : IRequest<ProductDto>
    {
        public Guid Id { get; set; }
    }
}
