using Application.DTOs.Requests;

namespace Application.UseCases.User.CreateUser;

public interface ICreateUserHandler
{
    Task<Guid> HandleAsync(CreateUserDto createUserDto);
}