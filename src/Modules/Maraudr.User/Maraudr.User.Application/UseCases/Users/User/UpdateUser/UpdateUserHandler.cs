using Application.DTOs.UsersQueriesDtos.Requests;
using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.UseCases.Users.User.UpdateUser;
public class UpdateUserHandler(IUserRepository repository):IUpdateUserHandler
{
    public async Task<Guid> HandleAsync(Guid id, UpdateUserDto updateUserDto)
    {
        var user = await repository.GetByIdAsync(id);
        if (user == null)
        {
            throw new InvalidOperationException($"L'utilisateur avec l'ID {id} n'existe pas.");
        }
        user.UpdateUserDetails(updateUserDto.Firstname,
            updateUserDto.Lastname,
            updateUserDto.Email,
            updateUserDto.PhoneNumber,
            updateUserDto.Street,
            updateUserDto.City,
            updateUserDto.State,
            updateUserDto.PostalCode,
            updateUserDto.Country,
            updateUserDto.Languages);
        
        await repository.UpdateAsync(user);
        return user.Id;
    }

}