using Maraudr.Domain.ValueObjects;

namespace Maraudr.User.Domain.Entities;

public abstract class AbstractUser
{
   

    public virtual AccountType AccountType { get; protected set; }
    public Guid Id { get;  set; }
    public string Firstname { get;  set; }
    public string Lastname { get;  set; }
    
    public DateTime CreatedAt { get; private set; } // TODO : comment ? 
    public DateTime LastLoggedIn { get;  set; }
    public bool IsActive { get;  set; }
    public ContactInfo ContactInfo { get;  set; }
    public Address Address { get;  set; } 
    
    private bool IsAdmin {
        get;
        set;
    }
    
    public string? Biography { get; set; }

    
    
    public List<Language> Languages { get; set; }


    protected AbstractUser( string firstname, string lastname, DateTime createdAt,
        ContactInfo contactInfo, Address address,List<Language> languages)
    {
        Id = new Guid();
        Firstname = firstname;
        Lastname = lastname;
        LastLoggedIn = DateTime.Now;
        CreatedAt = createdAt;
        IsActive = true;
        ContactInfo = contactInfo;
        Address = address;
        Languages = languages;
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
    
    private AbstractUser() { }

    public bool isUserManager()
    {
        return AccountType.Manager == AccountType;
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
    
    public AbstractUser ChangeUserRole(AbstractUser targetUser, AccountType newRole,AbstractUser newManager)
    {
        if (!this.IsAdmin)
            throw new UnauthorizedAccessException("Only admins can change user roles");
            
        // TODO : peut on changer le rôle d'un autre admin? 
        if (targetUser.IsAdmin && targetUser.Id != this.Id)
            throw new UnauthorizedAccessException("Cannot change the role of another admin");
            
        AccountType oldRole = targetUser.AccountType;
        
        if (oldRole == AccountType.Member && newRole == AccountType.Manager)
        {
            return ConvertUserToManager((User)targetUser);
        }
        else if (oldRole == AccountType.Manager && newRole == AccountType.Member)
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
        if (!newManager.isUserManager())
        {
            throw new ArgumentException("New manager should be declared manager efore");
        }

        var cNewManager = (Manager)newManager;
        List<AbstractUser> team = cNewManager.Team;
        cNewManager.AddMembersToTeam(team);
        return new User(manager.Id,manager.Firstname, manager.Lastname, manager.CreatedAt, manager.ContactInfo, manager.Address,
            manager.Languages,cNewManager);
        
    }
    

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType() && Equals((AbstractUser)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}