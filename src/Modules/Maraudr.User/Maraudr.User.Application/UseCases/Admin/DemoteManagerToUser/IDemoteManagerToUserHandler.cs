
public interface IDemoteManagerToUserHandler
{
    Task HandleAsync(Guid targetUserId);

}