using Application.DTOs.UsersQueriesDtos.Requests;

namespace Application.UseCases.Users.User.CreateUser;

public interface ICreateUserHandler
{
    Task<Guid> HandleAsync(CreateUserDto createUserDto);
}