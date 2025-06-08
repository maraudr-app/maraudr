using Application.DTOs.DisponibilitiesQueriesDtos.Requests;
using FluentValidation;
using Maraudr.User.Domain.ValueObjects.Users;

namespace Application.Validators;

public class CreateDisponiblityValidator : AbstractValidator<CreateDisponiblityRequest>
{
    public CreateDisponiblityValidator()
    {
        RuleFor(d => d.Start)
            .NotEmpty().WithMessage("La date de début est obligatoire")
            .Must(BeInFuture).WithMessage("La date de début doit être dans le futur");
        
        RuleFor(d => d.End)
            .NotEmpty().WithMessage("La date de fin est obligatoire")
            .GreaterThan(d => d.Start).WithMessage("La date de fin doit être postérieure à la date de début");
        
        RuleFor(d => d)
            .Must(BeLogicalDuration).WithMessage("La durée de disponibilité doit être raisonnable (moins de 120h)");
        
        
    
    }
    
    private bool BeInFuture(DateTime dateTime)
    {
        return dateTime > DateTime.UtcNow;
    }
    
    private bool BeLogicalDuration(CreateDisponiblityRequest request)
    {
        TimeSpan duration = request.End - request.Start;
        return duration.TotalHours <= 120;
    }
    

}