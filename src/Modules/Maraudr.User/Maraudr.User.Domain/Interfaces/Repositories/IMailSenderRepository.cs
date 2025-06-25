namespace Maraudr.User.Domain.Interfaces.Repositories;

public interface IMailSenderRepository
{
    public Task SendWelcomeEmailTo(string email, string name);

    public Task SendResetEmailAsync(string name,string email,  string token);

    public Task SendInvitationEmailAsync(string invitingUser, string AssociationName,string invitedEmail, string token, string message);

}