namespace Application.UseCases.Users.Admin.GrantAdminPrivileges;

public interface IGrantAdminPrivilegesHandler
{
    Task HandleAsync(Guid targetUserId);

}