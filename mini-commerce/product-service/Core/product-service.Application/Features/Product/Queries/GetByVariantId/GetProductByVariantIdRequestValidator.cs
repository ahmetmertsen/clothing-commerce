using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace product_service.Application.Features.Product.Queries.GetByVariantId
{
    public class GetProductByVariantIdRequestValidator : AbstractValidator<GetProductByVariantIdRequest>
    {
        public GetProductByVariantIdRequestValidator() 
        {
            RuleFor(x => x.VariantId)
                .NotEmpty().WithMessage("Ürün varyantı bilgisi boş olamaz.");
        }
    }
}
