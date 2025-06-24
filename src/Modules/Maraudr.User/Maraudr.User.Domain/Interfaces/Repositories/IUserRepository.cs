

using Maraudr.User.Domain.Entities.Tokens;
using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Domain.ValueObjects.Users;

namespace Maraudr.User.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<AbstractUser?> GetByIdAsync(Guid id);
    Task<IEnumerable<AbstractUser>> GetAllAsync();
    Task AddAsync(AbstractUser user);
    //Task<User?> UpdateAsync(User user);
    Task DeleteAsync(AbstractUser user);

    Task UpdateAsync(AbstractUser user);
    Task<IEnumerable<AbstractUser>> SearchByNameAsync(string searchTerm);
    Task<AbstractUser?> GetByEmailAsync(string email);
    Task<IEnumerable<Disponibility>> GetDisponibilitiesByAssociationIdAsync(Guid associationId);
    Task InvalidateExistingTokensAsync(Guid userId);

    Task SaveResetToken(PasswordResetToken token);

    Task<bool> ValidateResetToken(string token);
    Task<PasswordResetToken?> GetResetToken(string token);

    Task UpdateResetPasswordTokenAsync(PasswordResetToken token);




}