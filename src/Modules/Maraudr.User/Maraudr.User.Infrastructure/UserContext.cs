using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Maraudr.Domain.Entities;
using Maraudr.Domain.ValueObjects;

namespace Maraudr.User.Infrastructure;

public class UserContext
{
    private readonly List<Domain.Entities.User> _users;

    public UserContext()
    {
        _users = GenerateMockUsers();
    }

    public async Task<Domain.Entities.User?> GetUserByIdAsync(Guid id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public async Task<IEnumerable<Domain.Entities.User>> GetAllUsersAsync()
    {
        return _users.ToList();
    }

    public async Task AddUserAsync(Domain.Entities.User user)
    {
        _users.Add(user);
    }

    public async Task UpdateUserAsync(Domain.Entities.User user)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser != null)
        {
            _users.Remove(existingUser);
            _users.Add(user);
        }
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user != null)
        {
            _users.Remove(user);
        }
    }

    private List<Domain.Entities.User> GenerateMockUsers()
    {
        return new List<Domain.Entities.User>
        {
            new Domain.Entities.User(
                Guid.Parse("8f7c2e9d-4b3a-4e5f-9c1d-8a7b6c5d4e3f"),
                "Jean",
                "Dupont",
                DateTime.Now.AddYears(-2),
                new ContactInfo("jean.dupont@example.com", "+33123456789"),
                new Address("123 Rue de Paris", "Paris", "Île-de-France", "75001", "France"),
                AccountType.Member,
                null,
                null
            ),
            
            new Domain.Entities.User(
                Guid.Parse("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d"),
                "Marie",
                "Laurent",
                DateTime.Now.AddYears(-1),
                new ContactInfo("marie.laurent@example.com", "+33698765432"),
                new Address("45 Avenue Victor Hugo", "Lyon", "Auvergne-Rhône-Alpes", "69002", "France"),
                AccountType.Manager,
                null,null
            ),
            
            new Domain.Entities.User(
                Guid.Parse("a1b2c3d4-e5f6-a7b8-c9d0-e1f2a3b4c5d6"),
                "Philippe",
                "Martin",
                DateTime.Now.AddMonths(-6),

                new ContactInfo("philippe.martin@example.com", "+33712345678"),
                new Address("8 Boulevard de la Liberté", "Lille", "Hauts-de-France", "59000", "France"),
                AccountType.Admin,null,null
            ),
            
            new Domain.Entities.User(
                Guid.Parse("9d8c7b6a-5f4e-3d2c-1b0a-9f8e7d6c5b4a"),
                "Sophie",
                "Bernard",
                DateTime.Now.AddMonths(-3),
                new ContactInfo("sophie.bernard@example.com", "+33687654321"),
                new Address("12 Rue Paradis", "Marseille", "Provence-Alpes-Côte d'Azur", "13001", "France"),
                AccountType.Member,null,null
            )
        };
    }
}