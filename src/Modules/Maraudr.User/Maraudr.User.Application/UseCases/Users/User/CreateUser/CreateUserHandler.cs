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
    if (createUserDto.IsManager)
    {
        var existingManager = await repository.GetByEmailAsync(createUserDto.Email);
        if (existingManager != null)
        {
            throw new InvalidOperationException($"L'email {createUserDto.Email} est déjà utilisé.");
        }
        
        var manager = CreationCommandToManager.MapCreationCommandToManager(createUserDto, passwordManager);
        await repository.AddAsync(manager);
        await mailSenderRepository.SendWelcomeEmailTo(manager.ContactInfo.Email, manager.Firstname);
        return manager.Id;
    }
    else
    {
        var managerId = await repository.GetManagerIdByInvitationTokenAsync(createUserDto.ManagerToken);
        var manager = await repository.GetByIdAsync(managerId);
        
        if (manager is not { Role: Role.Manager })
        {
            throw new InvalidOperationException($"Le manager avec le token {createUserDto.ManagerToken} n'existe pas.");
        }

        var cManager = (Maraudr.User.Domain.Entities.Users.Manager)manager;
        var existingUser = await repository.GetByEmailAsync(createUserDto.Email);

        // Si l'utilisateur existe déjà
        if (existingUser != null)
        {
            // Vérifier s'il n'est pas déjà dans l'équipe
            if (cManager.Team.Contains(existingUser))
            {
                throw new InvalidOperationException("Vous êtes déjà membre de cette équipe.");
            }
            
            // L'ajouter à la nouvelle équipe
            cManager.AddMemberToTeam(existingUser);
            await repository.UpdateAsync(cManager);
            await repository.InvalidateExistingInvitationsAsync(createUserDto.Email);
            return existingUser.Id;
        }
        
        // Créer un nouvel utilisateur
        var newUser = CreationCommandToUser.MapCreationCommandToUser(createUserDto, manager, passwordManager);
        if (newUser == null)
        {
            throw new InvalidOperationException("Erreur lors de la création de l'utilisateur.");
        }
        
        cManager.AddMemberToTeam(newUser);
        await repository.AddAsync(newUser);
        await repository.UpdateAsync(cManager);
        await repository.InvalidateExistingInvitationsAsync(createUserDto.Email);
        await mailSenderRepository.SendWelcomeEmailTo(newUser.ContactInfo.Email, newUser.Firstname);
        return newUser.Id;
    }
}
}

