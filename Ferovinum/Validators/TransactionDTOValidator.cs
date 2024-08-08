using Ferovinum.Domain;
using Ferovinum.Services.DTO;
using FluentValidation;

namespace Ferovinum.Validators
{
    public class TransactionDTOValidator : AbstractValidator<TransactionDTO>
    {
        public TransactionDTOValidator()
        {
            RuleFor(dto => dto.ClientId).NotEmpty().MaximumLength(10);
            RuleFor(dto => dto.ProductId).NotEmpty().MaximumLength(10);
            RuleFor(dto => dto.Timestamp).NotEmpty();
            RuleFor(dto => dto.Quantity).NotEmpty().GreaterThan(0);
            RuleFor(dto => dto.OrderType).IsEnumName(typeof(OrderType), caseSensitive: false);
        }
    }
}

