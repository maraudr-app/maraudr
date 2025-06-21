using FluentValidation;
using Maraudr.Planning.Application.DTOs;

namespace Maraudr.Planning.Application.Validators;


public class CreateEventDtoValidator : AbstractValidator<CreateEventDto>
{
    public CreateEventDtoValidator()
    {
        RuleFor(x => x.AssociationId)
            .NotEmpty().WithMessage("L'identifiant de l'association est requis.");

        RuleFor(x => x.ParticipantsIds)
            .NotNull().WithMessage("La liste des participants ne peut pas être nulle.")
            .Must(x => x.Count > 0).WithMessage("Il doit y avoir au moins un participant.");

        RuleFor(x => x.BeginningDate)
            .NotEmpty().WithMessage("La date de début est requise.")
            .LessThan(x => x.EndDate).WithMessage("La date de début doit être antérieure à la date de fin.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("La date de fin est requise.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Le titre est requis.")
            .MaximumLength(100).WithMessage("Le titre ne peut pas dépasser 100 caractères.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La description ne peut pas dépasser 500 caractères.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("L'emplacement est requis.")
            .MaximumLength(200).WithMessage("L'emplacement ne peut pas dépasser 200 caractères.");
    }
}