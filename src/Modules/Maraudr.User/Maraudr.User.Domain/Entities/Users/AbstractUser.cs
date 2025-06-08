using System.ComponentModel.DataAnnotations;
using Maraudr.User.Domain.ValueObjects.Users;

namespace Maraudr.User.Domain.Entities.Users;

public abstract class AbstractUser
{
    public virtual Role Role { get; protected set; }
    public Guid Id { get;  set; }
    public string Firstname { get;  set; }
    public string Lastname { get;  set; }
    
    public DateTime CreatedAt { get; private set; } // TODO : comment ? 
    public DateTime LastLoggedIn { get;  set; }
    public bool IsActive { get;  set; }
    public ContactInfo ContactInfo { get;  set; }
    public Address Address { get;  set; } 
    public string PasswordHash { get; set; }

    public virtual ICollection<Disponibility> Disponibilities { get; private set; } = new List<Disponibility>();
    private bool IsAdmin {
        get;
        set;
    }
    
    public string? Biography { get; set; }

    public string UserType { get; private set; } = null!;

    
    public List<Language> Languages { get; set; }
    [Timestamp]
    public byte[] RowVersion { get; set; }

    protected AbstractUser( string firstname, string lastname, DateTime createdAt,
        ContactInfo contactInfo, Address address,List<Language> languages,string passwordHash)
    {
        Id = new Guid();
        Firstname = firstname;
        Lastname = lastname;
        LastLoggedIn = DateTime.Now;
        CreatedAt = createdAt;
        IsActive = false;
        ContactInfo = contactInfo;
        Address = address;
        Languages = languages;
        PasswordHash = passwordHash;
    }
    protected AbstractUser( Guid id,string firstname, string lastname, DateTime createdAt,
        ContactInfo contactInfo, Address address,List<Language> languages)
    {
        Id = id;
        Firstname = firstname;
        Lastname = lastname;
        LastLoggedIn = DateTime.Now;
        CreatedAt = createdAt;
        IsActive = true;
        ContactInfo = contactInfo;
        Address = address;
        Languages = languages;
    }
    
    protected AbstractUser() { }

    public bool IsUserManager()
    {
        return Role.Manager == Role;
    }
    
    // Admin section 
    public bool IsUserAdmin()
    {
        return IsAdmin;
    }
    public void GrantAdminPrivileges(AbstractUser grantedBy)
    {
        if (!grantedBy.IsAdmin)
            throw new UnauthorizedAccessException("Only admins can grant admin privileges");
            
        IsAdmin = true;
    }
    
    public void RevokeAdminPrivileges(AbstractUser revokedBy)
    {
        if (!revokedBy.IsAdmin)
            throw new UnauthorizedAccessException("Only admins can revoke admin privileges");
            
        // TODO : Get Association Admins : If Admins. count = 0 can't remove 
        // 
        
        IsAdmin = false;
    }
    
    public AbstractUser ChangeUserRole(AbstractUser targetUser, Role newRole,AbstractUser newManager)
    {
        if (!this.IsAdmin)
            throw new UnauthorizedAccessException("Only admins can change user roles");
            
        // TODO : peut on changer le rôle d'un autre admin? 
        if (targetUser.IsAdmin && targetUser.Id != this.Id)
            throw new UnauthorizedAccessException("Cannot change the role of another admin");
            
        Role oldRole = targetUser.Role;
        
        if (oldRole == Role.Member && newRole == Role.Manager)
        {
            return ConvertUserToManager((User)targetUser);
        }
        else if (oldRole == Role.Manager && newRole == Role.Member)
        {
            return ConvertManagerToUser((Manager)targetUser,newManager );
        }
        
        return targetUser;
    }
    
    private AbstractUser ConvertUserToManager(User user )
    {
        return new Manager(user.Id,user.Firstname, user.Lastname, user.CreatedAt, user.ContactInfo, user.Address,
            user.Languages, new List<AbstractUser>());
    }

    private AbstractUser ConvertManagerToUser(Manager manager,AbstractUser newManager )
    {
        if (!newManager.IsUserManager())
        {
            throw new ArgumentException("New manager should be declared manager efore");
        }

        var cNewManager = (Manager)newManager;
        List<AbstractUser> team = cNewManager.Team;
        cNewManager.AddMembersToTeam(team);
        return new User(manager.Id,manager.Firstname, manager.Lastname, manager.CreatedAt, manager.ContactInfo, manager.Address,
            manager.Languages,cNewManager);
        
    }

    public void UpdateUserDetails(string? firstname, string? lastname,
        string? email, string? phoneNumber, string? street, string? city, string? state, string? postalCode,
        string? country,List<string>? languages)
    {
        Firstname = firstname ?? Firstname;
        Lastname = lastname ?? Lastname;
        
        if (email != null || phoneNumber != null)
        {
            var contactInfo = new ContactInfo(
                email ?? ContactInfo.Email,
                phoneNumber ?? ContactInfo.PhoneNumber
            );
            ContactInfo = contactInfo;
        }
        if (street != null || city != null || state != null || postalCode != null || country != null)
        {
            Address = new Address(
                street ?? Address.GetStreet(),
                city ?? Address.GetCity(),
                state ?? Address.GetState(),
                postalCode ?? Address.GetPostalCode(),
                country ?? Address.GetCountry()
            );
        }
        if (languages != null)
        {
            Languages = languages
                .Select(l => (Language)Enum.Parse(typeof(Language), l, true))
                .ToList();
        }
        
    }

    public void SetUserStatus(bool isConnected)
    {
        if (isConnected)
        {
            LastLoggedIn = DateTime.Now;

        }

        IsActive = isConnected;
    }

    // Méthode pour ajouter une disponibilité
    public void AddDisponiblity(DateTime start, DateTime end)
    {
        if (start >= end)
            throw new ArgumentException("La date de début doit être antérieure à la date de fin");
    
        var newDisponiblity = new Disponibility { Start = start, End = end, UserId = Id };
    
        if (Disponibilities.Any(d => d.Overlaps(newDisponiblity)))
            throw new InvalidOperationException("Cette disponibilité chevauche une disponibilité existante");
        
        Disponibilities.Add(newDisponiblity);
    }
    
    public void RemoveAvailability(Guid availabilityId)
    {
        var availability = Disponibilities.FirstOrDefault(a => a.Id == availabilityId);
        if (availability != null)
            Disponibilities.Remove(availability);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
    
        // Cast and compare by ID
        var other = (AbstractUser)obj;
        return Id.Equals(other.Id);
    }
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}