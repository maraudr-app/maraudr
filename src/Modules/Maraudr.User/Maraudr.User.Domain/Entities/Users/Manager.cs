using System.ComponentModel.DataAnnotations.Schema;
using Maraudr.User.Domain.ValueObjects.Users;

namespace Maraudr.User.Domain.Entities.Users;

public class Manager : AbstractUser
{

    [NotMapped]
    public List<AbstractUser> Team
    {
        get => EFTeam.Cast<AbstractUser>().ToList();
        set => EFTeam = value.Cast<User>().ToList();
    }

    public List<User> EFTeam { get; set; } = new();

    public Manager(
        string firstname,
        string lastname,
        DateTime createdAt,
        ContactInfo contactInfo,
        Address address,
        List<Language> languages,
        List<AbstractUser> teamMembers,
        string passwordHash
    )
        : base(firstname, lastname, createdAt, contactInfo, address, languages, passwordHash)
    {
        Team = teamMembers;
        Role = Role.Manager;
    }

    public Manager(
        Guid id,
        string firstname,
        string lastname,
        DateTime createdAt,
        ContactInfo contactInfo,
        Address address,
        List<Language> languages,
        List<AbstractUser> teamMembers
    )
        : base(id, firstname, lastname, createdAt, contactInfo, address, languages)
    {
        Team = teamMembers;
        Role = Role.Manager;

    }

    public Manager() { }

    public AbstractUser? GetTeamMember(AbstractUser member)
    {
        return Team.FirstOrDefault(m => m.Equals(member));
    }

    public void RemoveMemberFromTeam(AbstractUser member)
    {
        var mem = GetTeamMember(member);
        if (mem == null)
            throw new ArgumentException("Member not in team");

        EFTeam.Remove((User)mem);
    }

    public void AddMemberToTeam(AbstractUser member)
    {
        if (member == null) throw new ArgumentNullException(nameof(member));
        if (EFTeam.Any(m => m.Equals(member)))
            throw new ArgumentException("Member already in team");

        EFTeam.Add((User)member);
    }

    public void AddMembersToTeam(List<AbstractUser> members)
    {
        if (members == null) throw new ArgumentNullException(nameof(members));
        EFTeam = EFTeam.Union(members.Cast<User>()).ToList();
    }
}
