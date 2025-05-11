using Application.DTOs.UsersQueriesDtos.Requests;
using Application.Mappers;
using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.UseCases.Users.User.CreateUser;


public class CreateUserHandler(IUserRepository repository) : ICreateUserHandler
{

    public async Task<Guid> HandleAsync(CreateUserDto createUserDto)
    {
        if (createUserDto.IsManager)
        {
            var manager = CreationCommandToManager.MapCreationCommandToManager(createUserDto);
            await repository.AddAsync(manager);
            return manager.Id;
        }
        else
        {
            if (!createUserDto.ManagerId.HasValue)
            { 
                throw new InvalidOperationException("Un utilisateur non-manager doit avoir un manager assigné.");
            }  
            var manager = await repository.GetByIdAsync(createUserDto.ManagerId.Value);
            if (manager == null)
            {
                throw new InvalidOperationException($"Le manager avec l'ID {createUserDto.ManagerId} n'existe pas.");
            }

            var user = CreationCommandToUser.MapCreationCommandToUser(createUserDto,manager);
            await repository.AddAsync(user);
            return user.Id;
        }
        
    }
}
