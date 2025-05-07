using Maraudr.User.Application.DTOs.Requests;

namespace Application.UseCases.User.UpdateUser;

public interface IUpdateUserHandler
{
    Task<Guid> HandleAsync(Guid id, UpdateUserDto updateUserDto);

}