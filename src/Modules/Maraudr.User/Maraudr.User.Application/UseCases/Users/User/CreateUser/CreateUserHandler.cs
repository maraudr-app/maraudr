using Application.DTOs.UsersQueriesDtos.Requests;
using Application.Mappers;
using Maraudr.User.Domain.Interfaces.Repositories;
using Maraudr.User.Domain.ValueObjects.Users;
using Maraudr.User.Infrastructure.Security;

namespace Application.UseCases.Users.User.CreateUser;


public class CreateUserHandler(IUserRepository repository,IPasswordManager passwordManager) : ICreateUserHandler
{

    public async Task<Guid> HandleAsync(CreateUserDto createUserDto)
    {
        if(repository.GetByEmailAsync(createUserDto.Email).Result != null)
        {
            throw new InvalidOperationException($"L'email {createUserDto.Email} est déjà utilisé.");
        }
        
        if (createUserDto.IsManager)
        {
            var manager = CreationCommandToManager.MapCreationCommandToManager(createUserDto,passwordManager);
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
            if (manager is not { Role: Role.Manager })
            {
                throw new InvalidOperationException($"Le manager avec l'ID {createUserDto.ManagerId} n'existe pas.");
            }

            var user = CreationCommandToUser.MapCreationCommandToUser(createUserDto,manager,passwordManager);
            await repository.AddAsync(user);
            return user.Id;
        }
        
    }
}
