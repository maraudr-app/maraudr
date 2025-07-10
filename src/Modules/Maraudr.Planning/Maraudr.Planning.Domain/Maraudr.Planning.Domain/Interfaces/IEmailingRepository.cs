namespace Maraudr.Planning.Domain.Interfaces;

public interface IEmailingRepository
{
    public Task SendEventEmailAsync(List<Guid> usersIds,string eventTitle,string eventDescription);

}