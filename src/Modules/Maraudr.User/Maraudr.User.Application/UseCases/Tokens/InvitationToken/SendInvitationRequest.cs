using System.Security.Cryptography;
using Application.DTOs.InvitationDto;
using Maraudr.User.Domain.Entities.Tokens;
using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.UseCases.Tokens;

public interface ISendInvitationRequestHandler
{
    Task HandleAsync(Guid invitedByUserId, SendInvitationRequest request);
}


public class SendInvitationRequestHandler(IAssociationRepository associationRepository, IMailSenderRepository mailSenderRepository, IUserRepository userRepository) : ISendInvitationRequestHandler
{
    public async Task HandleAsync(Guid invitedByUserId, SendInvitationRequest request)
    { 
        
            var existingUser = await userRepository.GetByIdAsync(invitedByUserId);
            if (existingUser == null)
            {
                throw new ArgumentException("Utilisateur non existant");
            }

            if (!existingUser.IsUserManager())
            {
                throw new ArgumentException($"Utilisateur {existingUser.Lastname}  {existingUser.Firstname }non existant");
            }

            var isMember = await associationRepository.IsUserMemberOfAssociationAsync(existingUser.Id, request.AssociationId);
            if (!isMember)
            {
                throw new ArgumentException(
                    $"{existingUser.Firstname} {existingUser.Lastname} is not member of association");
            }

            var associationName = await associationRepository.GetAssociationName(request.AssociationId);

            await userRepository.InvalidateExistingInvitationsAsync(request.InvitedEmail);

            
            var invitation = new InvitationToken
            {
                InvitedByUserId = invitedByUserId,
                InvitedEmail = request.InvitedEmail,
                AssociationId = request.AssociationId,
                Token = GenerateSecureToken(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7), 
                IsUsed = false,
            };
            
            await userRepository.AddInvitationToken(invitation);

            await mailSenderRepository.SendInvitationEmailAsync(existingUser.Firstname+" "+existingUser.Lastname, associationName, request.InvitedEmail, invitation.Token, request.Message);
        }
    
        private string GenerateSecureToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[64];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

    }
