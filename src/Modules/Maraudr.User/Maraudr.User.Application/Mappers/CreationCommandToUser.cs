using Application.DTOs.Requests;
using Maraudr.User.Application.DTOs.Requests;
using Maraudr.User.Domain.Entities;
using Maraudr.User.Domain.ValueObjects;

namespace Application.Mappers;

public class CreationCommandToUser
{
    public static User MapCreationCommandToUser(CreateUserDto createUserDto, AbstractUser manager)
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



        return new User(
            createUserDto.Firstname,
            createUserDto.Lastname,
            DateTime.Now,
            contact,
            address,
            domainLanguages,
            manager 
        );
    }
}