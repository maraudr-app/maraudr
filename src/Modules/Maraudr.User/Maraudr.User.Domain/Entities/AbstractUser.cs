using Maraudr.Domain.ValueObjects;

namespace Maraudr.Domain.Entities;

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
    
    public string? Biography { get; set; }

    
    
    public List<Language> Languages { get; set; }


    protected AbstractUser(Guid id, string firstname, string lastname, DateTime createdAt,
        ContactInfo contactInfo, Address address, AccountType accountType,List<Language> languages)
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
}