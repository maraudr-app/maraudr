namespace Application.UseCases.Users.User.LogoutUser;

public interface ILogoutUserHandler
{
    public Task HandleAsync(Guid currentUSerId);
}