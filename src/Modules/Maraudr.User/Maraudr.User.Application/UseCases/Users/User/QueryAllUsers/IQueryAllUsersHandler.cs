using Maraudr.User.Domain.Entities.Users;

namespace Application.UseCases.Users.User.QueryAllUsers
{
    public interface IQueryAllUsersHandler
    {
        public Task<IEnumerable<AbstractUser>> HandleAsync();
    } 
}

