/*


using AutoMapper;

using Maraudr.User.Application.DTOs.Requests;
using Maraudr.User.Application.DTOs.Responses;
using Maraudr.User.Application.Exceptions;
using Maraudr.User.Domain.Entities;
using Maraudr.User.Domain.Interfaces;
using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.Services;

public class UserService 
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<AbstractUser?> GetUserByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
        
    }

    public async Task<IEnumerable<AbstractUser?>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    


    public async Task<Guid> CreateUserAsync(CreateUserDto userCommand)
    {
        var user = _mapper.Map<User>(userCommand);
        await _userRepository.AddAsync(user);
        return user.Id;
    }

   /* public async Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);    
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        _mapper.Map(updateUserDto, user);
        var updatedUser = await _userRepository.UpdateAsync(user);
        return _mapper.Map<UserDto>(updatedUser);
    }

    public async Task DeleteUserAsync(Guid userId)
    { 
        var user = await _userRepository.GetByIdAsync(userId);    
       /* if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        await _userRepository.DeleteAsync(user);
    }
    

} */