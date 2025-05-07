namespace Application.UseCases.Manager.RemoveUserFromManagersTEam;

public interface IRemoveUserFromManagerTeamHandler
{
    public Task HandleAsync(Guid managerId, Guid userId);
}