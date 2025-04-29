using FluentValidation;
using Maraudr.Stock.Application.UseCases;

namespace Maraudr.Stock.Application.Validation;

public class ItemValidator : AbstractValidator<CreateItemCommand>
{
    public ItemValidator()
    {
        RuleFor(item => item.Name).NotEmpty().WithMessage("Item name is required");
        RuleFor(item => item.ItemType).IsInEnum().WithMessage("Invalid category");
    }
}
