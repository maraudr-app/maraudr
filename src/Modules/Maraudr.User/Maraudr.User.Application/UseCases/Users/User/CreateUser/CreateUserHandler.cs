using Application.DTOs.UsersQueriesDtos.Requests;
using Application.Mappers;
using Maraudr.User.Domain.Interfaces.Repositories;
using Maraudr.User.Domain.ValueObjects.Users;
using Maraudr.User.Infrastructure.Security;

namespace Application.UseCases.Users.User.CreateUser;


public class CreateUserHandler(IUserRepository repository,IPasswordManager passwordManager,IMailSenderRepository mailSenderRepository ) : ICreateUserHandler
{

    public async Task<Guid> HandleAsync(CreateUserDto createUserDto)
    {
        if(await repository.GetByEmailAsync(createUserDto.Email) != null)
        {
            throw new InvalidOperationException($"L'email {createUserDto.Email} est déjà utilisé.");
        }
        if (createUserDto.IsManager)
        {
            var manager = CreationCommandToManager.MapCreationCommandToManager(createUserDto,passwordManager);
            await repository.AddAsync(manager);
            await mailSenderRepository.SendWelcomeEmailTo(manager.ContactInfo.Email, manager.Firstname);
            return manager.Id;
        }else{
            var managerId = await repository.GetManagerIdByInvitationTokenAsync(createUserDto.ManagerToken);
            var manager = await repository.GetByIdAsync(managerId);
            if (manager is not { Role: Role.Manager })
            {
                throw new InvalidOperationException($"Le manager avec le token {createUserDto.ManagerToken} n'existe pas.");
            }
            

            var user = CreationCommandToUser.MapCreationCommandToUser(createUserDto,manager,passwordManager);
            if (user == null)
            {
                throw new InvalidOperationException("Erreur lors de la création de l'utilisateur.");
            }
            await repository.AddAsync(user);
            var cManager = (Maraudr.User.Domain.Entities.Users.Manager)manager;
            cManager.AddMemberToTeam(user);
            await repository.UpdateAsync(cManager);

            await mailSenderRepository.SendWelcomeEmailTo(user.ContactInfo.Email, user.Firstname);
            return user.Id;
        }
        
        
    }
}
