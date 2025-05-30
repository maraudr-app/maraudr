using FluentValidation;
using Maraudr.Associations.Application.Dtos;
using Maraudr.Associations.Application.Validation;

public class UpdateAssociationInformationDtoValidator : AbstractValidator<UpdateAssociationInformationDto>
{
    public UpdateAssociationInformationDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100);

        RuleFor(x => x.AddressDto)
            .NotNull().WithMessage("Address is required.")
            .SetValidator(new AddressDtoValidator());
    }
}