namespace Application.UseCases.Users.Manager.RemoveUserFromManagersTeam;

public interface IRemoveUserFromManagerTeamHandler
{
    public Task HandleAsync(Guid managerId, Guid userId);
}