namespace  Application.UseCases.Manager.AddUserToManagersTeam;

public interface IAddUserToManagersTeamHandler
{
    public Task HandleAsync(Guid id,Guid userId);
}