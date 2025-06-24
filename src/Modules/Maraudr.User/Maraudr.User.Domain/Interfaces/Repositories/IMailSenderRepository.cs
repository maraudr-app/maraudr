namespace Maraudr.User.Domain.Interfaces.Repositories;

public interface IMailSenderRepository
{
    public Task SendWelcomeEmailTo(string email, string name);

    public Task SendResetEmailAsync(string name,string email,  string token);
}