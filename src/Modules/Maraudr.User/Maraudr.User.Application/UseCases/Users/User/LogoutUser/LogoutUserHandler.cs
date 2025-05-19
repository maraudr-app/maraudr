    using Application.UseCases.Users.User.LogoutUser;
    using Maraudr.User.Domain.Entities.Tokens;
    using Maraudr.User.Domain.Interfaces.Repositories;

    namespace Application.UseCases.Users.User.LogoutUser;

    public class LogoutUserHandler(IUserRepository repository, IRefreshTokenRepository refreshRepository):ILogoutUserHandler
    {
        public async Task HandleAsync(Guid currentUserId)
        {
            var refreshTokens = await refreshRepository.GetActiveRefreshTokensByUserIdAsync(currentUserId);
            var currentUser = await repository.GetByIdAsync(currentUserId);
            currentUser.SetUserStatus(false);
            await repository.UpdateAsync(currentUser);

            foreach(var r in refreshTokens)
            {
                r.Revoke("Logout");
                refreshRepository.UpdateAsync(r);
            }
            

            
        }
    }