using Maraudr.User.Application.DTOs.Responses;

namespace Application.UseCases.Users.User.SearchByNameUser;

public interface ISearchByNameUserHandler
{
    Task<IEnumerable<UserDto>> HandleAsync(string searchTerm);
}