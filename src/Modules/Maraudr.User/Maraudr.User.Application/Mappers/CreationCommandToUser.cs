using Application.DTOs.UsersQueriesDtos.Requests;
using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Domain.ValueObjects.Users;
using Maraudr.User.Infrastructure.Security;

namespace Application.Mappers;

public static class CreationCommandToUser
{
    public static User MapCreationCommandToUser(CreateUserDto createUserDto, AbstractUser manager,IPasswordManager passwordManager)
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
            manager ,
            passwordManager.HashPassword(createUserDto.Password) 
            
            
        );
    }
}