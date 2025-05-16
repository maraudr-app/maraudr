using Maraudr.User.Application.DTOs.Responses;
using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.UseCases.Users.User.SearchByNameUser;

// Application/UseCases/User/SearchByName/SearchByNameUserHandler.cs

public class SearchByNameUserHandler(IUserRepository repository) : ISearchByNameUserHandler
{
    public async Task<IEnumerable<UserDto>> HandleAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Enumerable.Empty<UserDto>();
            
        // Recherche insensible à la casse et partielle
        var matchingUsers = await repository.SearchByNameAsync(searchTerm.Trim());
        return matchingUsers.Select(u => new UserDto
        {
            Id = u.Id,
            Lastname = u.Lastname,
            Firstname = u.Firstname,
        });
    }
}