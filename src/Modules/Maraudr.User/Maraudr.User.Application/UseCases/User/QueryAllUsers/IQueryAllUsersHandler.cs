using Maraudr.User.Domain.Entities;

namespace Application.UseCases.User.QueryAllUsers
{
    public interface IQueryAllUsersHandler
    {
        public Task<IEnumerable<AbstractUser>> HandleAsync();
    } 
}

