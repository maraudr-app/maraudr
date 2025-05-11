using Application.DTOs.UsersQueriesDtos.Requests;

namespace Application.UseCases.Users.User.UpdateUser;

public interface IUpdateUserHandler
{
    Task<Guid> HandleAsync(Guid id, UpdateUserDto updateUserDto);

}