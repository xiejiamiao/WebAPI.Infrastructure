using FluentValidation;
using WebAPI.Infrastructure.ResourceModel.OrderResource;

namespace WebAPI.Infrastructure.ResourceModel.Validator
{
    public class OrderAddResourceValidator:AbstractValidator<OrderAddResource>
    {
        public OrderAddResourceValidator()
        {
            RuleFor(x => x.OrderNo)
                .NotNull()
                .WithName("订单号")
                .WithMessage("Required|{PropertyName}是必填的")
                .MaximumLength(50)
                .WithMessage("{PropertyName}的最大长度是{MaxLength}");
        }
    }
}