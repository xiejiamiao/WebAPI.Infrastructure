using FluentValidation;
using WebAPI.Infrastructure.ResourceModel.OrderResource;

namespace WebAPI.Infrastructure.ResourceModel.Validator
{
    public class OrderUpdateResourceValidator:AbstractValidator<OrderUpdateResource>
    {
        public OrderUpdateResourceValidator()
        {
            RuleFor(x => x.OrderNo)
                .NotNull()
                .WithName("订单号")
                .WithMessage("{PropertyName}不可为空");
        }
    }
}