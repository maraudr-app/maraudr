namespace Application.UseCases.Users.User.DeleteUser;

public interface IDeleteUserHandler
{
    public Task HandleAsync(Guid id);
}