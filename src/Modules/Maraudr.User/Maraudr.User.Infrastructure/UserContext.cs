using Maraudr.Domain.ValueObjects;
using Maraudr.User.Domain.Entities;

public class UserContext
{
    private readonly List<AbstractUser> _users;
    private readonly List<Manager> _managers;

    public UserContext()
    {
        _managers = new List<Manager>();
        _users = new List<AbstractUser>();
        
        // Générer d'abord les managers
        GenerateManagers();
        
        // Puis générer les utilisateurs avec des références aux managers
        GenerateUsers();
    }

    public async Task<AbstractUser?> GetUserByIdAsync(Guid id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public async Task<IEnumerable<AbstractUser>> GetAllUsersAsync()
    {
        return _users.ToList();
    }

    public async Task AddUserAsync(AbstractUser user)
    {
        _users.Add(user);
        
        // Si c'est un manager, l'ajouter aussi à la liste des managers
        if (user is Manager manager)
        {
            _managers.Add(manager);
        }
    }

    public async Task UpdateUserAsync(AbstractUser user)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser != null)
        {
            _users.Remove(existingUser);
            _users.Add(user);
            
            // Mettre à jour la liste des managers si nécessaire
            if (existingUser is Manager existingManager)
            {
                _managers.Remove(existingManager);
                if (user is Manager updatedManager)
                {
                    _managers.Add(updatedManager);
                }
            }
            else if (user is Manager newManager)
            {
                _managers.Add(newManager);
            }
        }
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user != null)
        {
            _users.Remove(user);
            
            // Supprimer de la liste des managers si nécessaire
            if (user is Manager manager)
            {
                _managers.Remove(manager);
            }
        }
    }

    private void GenerateManagers()
    {
        var manager1 = new Manager(
            "Marie",
            "Laurent",
            DateTime.Now.AddYears(-1),
            new ContactInfo("marie.laurent@example.com", "+33698765432"),
            new Address("45 Avenue Victor Hugo", "Lyon", "Auvergne-Rhône-Alpes", "69002", "France"),
            new List<Language> { Language.English },
            new List<AbstractUser>()
        );
        
        var manager2 = new Manager(
            "Sophie",
            "Bernard",
            DateTime.Now.AddMonths(-3),
            new ContactInfo("sophie.bernard@example.com", "+33687654321"),
            new Address("12 Rue Paradis", "Marseille", "Provence-Alpes-Côte d'Azur", "13001", "France"),
            new List<Language> { Language.French, Language.German },
            new List<AbstractUser>()
        );
        
        _managers.Add(manager1);
        _managers.Add(manager2);
        _users.Add(manager1);
        _users.Add(manager2);
    }

    private void GenerateUsers()
    {
        if (_managers.Count == 0)
        {
            throw new InvalidOperationException("Au moins un manager doit être créé avant les utilisateurs");
        }
        
        var defaultManager = _managers[0]; 
        
        var user1 = new User(
            "Jean",
            "Dupont",
            DateTime.Now.AddYears(-2),
            new ContactInfo("jean.dupont@example.com", "+33123456789"),
            new Address("123 Rue de Paris", "Paris", "Île-de-France", "75001", "France"),
            new List<Language> { Language.English, Language.German },
            defaultManager  // Spécifier un manager existant
        );
        
        var user2 = new User(
            "Pierre",
            "Martin",
            DateTime.Now.AddMonths(-6),
            new ContactInfo("pierre.martin@example.com", "+33612345678"),
            new Address("8 Rue du Commerce", "Toulouse", "Occitanie", "31000", "France"),
            new List<Language> { Language.French },
            _managers[1]  // Utiliser le deuxième manager
        );
        
        _users.Add(user1);
        _users.Add(user2);
        
        // Ajouter les utilisateurs aux équipes de leurs managers respectifs
        defaultManager.AddMemberToTeam(user1);
        _managers[1].AddMemberToTeam(user2);
    }
}