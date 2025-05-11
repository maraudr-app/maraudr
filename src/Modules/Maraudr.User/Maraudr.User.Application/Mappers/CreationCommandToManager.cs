using Application.DTOs.UsersQueriesDtos.Requests;
using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Domain.ValueObjects.Users;

namespace Application.Mappers;

public static class CreationCommandToManager
{
    public static Manager MapCreationCommandToManager(CreateUserDto createUserDto)
    {
        var contact = new ContactInfo(createUserDto.Email, createUserDto.PhoneNumber);
        var address = new Address(createUserDto.Street,
            createUserDto.City,
            createUserDto.State,
            createUserDto.PostalCode, createUserDto.Country);
        var languages = createUserDto.Languages;

        var domainLanguages = languages
            .Select(l => Enum.Parse<Language>(l, ignoreCase: true))
            .ToList();



        return new Manager(
            createUserDto.Firstname,
            createUserDto.Lastname,
            DateTime.Now,
            contact,
            address,
            domainLanguages,
            []
        );
    }
}