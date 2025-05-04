using Maraudr.User.Domain.Entities;
using Maraudr.User.Domain.ValueObjects;

namespace Maraudr.User.Domain.Entities;

public class Manager:AbstractUser
{
    
    
    public override Role Role { get; protected set; } = Role.Manager;
    public List<AbstractUser> Team{set; get; }
    
    public Manager( string firstname,  string lastname, DateTime createdAt,
                ContactInfo contactInfo, Address address, List<Language> languages,List<AbstractUser> teamMembers)
                : base(firstname, lastname, createdAt, contactInfo, address, languages)
    {
        this.Team = teamMembers;
    }
    
    public Manager( Guid id, string firstname,  string lastname, DateTime createdAt,
        ContactInfo contactInfo, Address address, List<Language> languages,List<AbstractUser> teamMembers)
        : base( id,firstname, lastname, createdAt, contactInfo, address, languages)
    {
        this.Team = teamMembers;
    }
    public Manager() { }

    public AbstractUser? GetTeamMember(AbstractUser member)
    {
        if (member == null) throw new ArgumentNullException(nameof(member));
        foreach (var m in Team)
        {
            if (m.Equals(member))
            {
                return m;
            }
        }
        return null;
    }

    public void RemoveMemberFromTeam(AbstractUser member)
    {
        if (member == null) throw new ArgumentNullException(nameof(member));
        var mem = GetTeamMember(member);
        if (mem == null)
        {
            throw new ArgumentException("Member not in team");
        }

        Team.Remove(mem);
    }

    public void AddMemberToTeam(AbstractUser member)
    {
        if (member == null) throw new ArgumentNullException(nameof(member));
        var mem = GetTeamMember(member);
        if (mem != null)
        {
            throw new ArgumentException("Member already in team");
        }   
        Team.Add(member);
    }
    //TODO : tester
    public void AddMembersToTeam(List<AbstractUser> members)
    {
        if (members == null) throw new ArgumentNullException(nameof(members));
        Team = Team.Union(members).ToList();
    }
}