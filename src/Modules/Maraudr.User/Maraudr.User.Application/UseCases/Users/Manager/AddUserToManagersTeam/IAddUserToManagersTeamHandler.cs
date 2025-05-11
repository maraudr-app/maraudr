namespace  Application.UseCases.Users.Manager.AddUserToManagersTeam;

public interface IAddUserToManagersTeamHandler
{
    public Task HandleAsync(Guid id,Guid userId);
}