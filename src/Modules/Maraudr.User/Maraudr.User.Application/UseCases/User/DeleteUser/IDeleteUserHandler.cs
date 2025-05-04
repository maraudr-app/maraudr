namespace Application.UseCases.User.DeleteUser;

public interface IDeleteUserHandler
{
    public Task HandleAsync(Guid id);
}