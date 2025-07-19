namespace Application.UseCases.Tokens;

using Application.DTOs;
using Maraudr.User.Domain.ValueObjects.Users;


using System.Security.Cryptography;
using Application.DTOs.InvitationDto;
using Maraudr.User.Domain.Entities.Tokens;
using Maraudr.User.Domain.Interfaces.Repositories;


public interface IValidateInvitationTokenHandler
{
    Task<InvitatonTokenDto> HandleAsync(string token);
}


public class ValidateInvitationTokenHandler(IUserRepository userRepository) : IValidateInvitationTokenHandler
{
    public async Task<InvitatonTokenDto> HandleAsync(string token)
    {

        var managerId = await userRepository.GetManagerIdByInvitationTokenAsync(token);
        var manager = await userRepository.GetByIdAsync(managerId);
        if (manager.Role != Role.Manager)
        {
            throw new InvalidOperationException($"Le manager avec le token {token} n'existe pas.");
        }
        var associationId = await userRepository.GetAssociationIdByInvitationTokenAsync(token);

        return new InvitatonTokenDto()
        {
            AssociationId = associationId,
            InvitedByFirstName = manager.Firstname,
            invitedByLastname = manager.Lastname
        };


    }
}