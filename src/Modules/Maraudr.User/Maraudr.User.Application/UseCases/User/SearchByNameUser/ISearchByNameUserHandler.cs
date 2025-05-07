using Maraudr.User.Application.DTOs.Responses;
using Maraudr.User.Domain.Entities;

namespace Application.UseCases.User.SearchByNameUser;

public interface ISearchByNameUserHandler
{
    Task<IEnumerable<UserDto>> HandleAsync(string searchTerm);
}