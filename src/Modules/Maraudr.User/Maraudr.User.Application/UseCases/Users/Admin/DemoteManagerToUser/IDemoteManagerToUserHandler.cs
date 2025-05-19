
namespace Application.UseCases.Users.Admin.DemoteManagerToUser;

public interface IDemoteManagerToUserHandler
{
    Task HandleAsync(Guid targetUserId);

}