using FluentValidation;
using Application.DTOs.UsersQueriesDtos.Requests;

namespace Application.Validators
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(user => user.Firstname).NotEmpty().WithMessage("Le prénom est obligatoire");
            RuleFor(user => user.Lastname).NotEmpty().WithMessage("Le nom est obligatoire");
            
            RuleFor(user => user.Email).NotEmpty().WithMessage("L'email est obligatoire")
                .EmailAddress().WithMessage("Format d'email invalide");
            
            RuleFor(user => user.PhoneNumber).NotEmpty().WithMessage("Le numéro de téléphone est obligatoire");
            
            RuleFor(user => user.Street).NotEmpty().WithMessage("La rue est obligatoire");
            RuleFor(user => user.City).NotEmpty().WithMessage("La ville est obligatoire");
            RuleFor(user => user.State).NotEmpty().WithMessage("La région est obligatoire");
            RuleFor(user => user.PostalCode).NotEmpty().WithMessage("Le code postal est obligatoire");
            RuleFor(user => user.Country).NotEmpty().WithMessage("Le pays est obligatoire");
            
            RuleFor(user => user.Languages).NotNull().WithMessage("La liste des langues ne peut pas être null");

            RuleFor(user => user.ManagerId)
                .NotNull().When(user => !user.IsManager)
                .WithMessage("Un manager doit être défini pour les utilisateurs non-managers");
            
        }
    }
}