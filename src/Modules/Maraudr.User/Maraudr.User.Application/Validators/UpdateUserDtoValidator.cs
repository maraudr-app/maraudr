using FluentValidation;
using Maraudr.User.Application.DTOs.Requests;

namespace Maraudr.User.Application.Validators;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {

        RuleFor(user => user.Email)
            .EmailAddress().When(user => !string.IsNullOrEmpty(user.Email))
            .WithMessage("Format d'email invalide");

        RuleFor(user => user)
            .Must(HaveAtLeastOneFieldToUpdate)
            .WithMessage("Au moins un champ à mettre à jour doit être présent");
    }
    
    private bool HaveAtLeastOneFieldToUpdate(UpdateUserDto dto)
    {
        return 
            !string.IsNullOrEmpty(dto.Firstname) ||
            !string.IsNullOrEmpty(dto.Lastname) ||
            !string.IsNullOrEmpty(dto.Email) ||
            !string.IsNullOrEmpty(dto.PhoneNumber) ||
            !string.IsNullOrEmpty(dto.Street) ||
            !string.IsNullOrEmpty(dto.City) ||
            !string.IsNullOrEmpty(dto.State) ||
            !string.IsNullOrEmpty(dto.PostalCode) ||
            !string.IsNullOrEmpty(dto.Country) ||
            (dto.Languages != null && dto.Languages.Any());
    }
}