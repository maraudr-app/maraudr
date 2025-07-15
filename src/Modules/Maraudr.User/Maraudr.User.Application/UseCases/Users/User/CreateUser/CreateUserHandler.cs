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
        var currentUser = await repository.GetByEmailAsync(createUserDto.Email);
       
        if (createUserDto.IsManager)
        {
            if( currentUser != null)
            {
                throw new InvalidOperationException($"L'email {createUserDto.Email} est déjà utilisé.");
            }
            
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

            if (currentUser == null)
            {
                currentUser = CreationCommandToUser.MapCreationCommandToUser(createUserDto,manager,passwordManager);
                if (currentUser == null)
                {
                    throw new InvalidOperationException("Erreur lors de la création de l'utilisateur.");
                }
            }
            
            var cManager = (Maraudr.User.Domain.Entities.Users.Manager)manager;
            if (cManager.Team.Contains(currentUser))
            {
                throw new InvalidOperationException($"Vous êtes déjà membre de l'équipe ");
 
            }
            cManager.AddMemberToTeam(currentUser);
            await repository.AddAsync(currentUser);
            await repository.UpdateAsync(cManager);
            await repository.InvalidateExistingInvitationsAsync(createUserDto.Email);
            await mailSenderRepository.SendWelcomeEmailTo(currentUser.ContactInfo.Email, currentUser.Firstname);
            return currentUser.Id;
        }
        
        
    }
}
