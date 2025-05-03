
using Application.DTOs.Requests;
using Application.Mappers;
using Maraudr.User.Application.DTOs.Requests;
using Maraudr.User.Domain.Interfaces.Repositories;
using Maraudr.User.Domain.Entities;

namespace Application.UseCases.User.CreateUser;


public class CreateUserHandler(IUserRepository repository) : ICreateUserHandler
{

    public async Task<Guid> HandleAsync(CreateUserDto createUserDto)
    {
        var manager = await repository.GetByIdAsync(createUserDto.ManagerId);
        if (manager == null)
        { 
            throw new InvalidOperationException($"Le manager avec l'ID {createUserDto.ManagerId} n'existe pas.");
        }
        var user = CreationCommandToUser.MapCreationCommandToUser(createUserDto,manager);
        await repository.AddAsync(user);
        return user.Id;
    }
}
