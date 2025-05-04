namespace Application.Services.Admin;

public interface IGrantAdminPrivilegesHandler
{
    Task HandleAsync(Guid targetUserId);

}