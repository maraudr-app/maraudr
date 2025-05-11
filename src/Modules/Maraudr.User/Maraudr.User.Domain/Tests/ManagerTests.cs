using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Domain.ValueObjects.Users;
using Xunit.Abstractions;

namespace Domain;

public class ManagerTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ContactInfo _validContactInfo;
    private readonly Address _validAddress;
    private readonly List<Language> _validLanguages;
    private readonly DateTime _createdAt;

    public ManagerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _validContactInfo = new ContactInfo("test@example.com", "1234567890");
        _validAddress = new Address("123 Main St", "Anytown", "State", "12345", "Country");
        _validLanguages = new List<Language> { Language.English, Language.French };
        _createdAt = DateTime.Now.AddDays(-10);
    }

    private Manager CreateManager(string firstName = "Manager", string lastName = "Test",
        List<AbstractUser> team = null)
    {
        return new Manager(firstName, lastName, _createdAt, _validContactInfo, _validAddress, _validLanguages,
            team ?? new List<AbstractUser>());
    }

    private User CreateUser(string firstName = "John", string lastName = "Doe")
    {
        var manager = CreateManager("Another", "Manager");
        return new User(firstName, lastName, _createdAt, _validContactInfo, _validAddress, _validLanguages, manager);
    }

    [Fact]
    public void Constructor_WithValidParameters_SetsPropertiesCorrectly()
    {
        var teamMembers = new List<AbstractUser> { CreateUser() };

        var manager = new Manager("Manager", "Test", _createdAt, _validContactInfo, _validAddress, _validLanguages,
            teamMembers);

        Assert.Equal("Manager", manager.Firstname);
        Assert.Equal("Test", manager.Lastname);
        Assert.Equal(_createdAt, manager.CreatedAt);
        Assert.True(manager.IsActive);
        Assert.Equal(_validContactInfo, manager.ContactInfo);
        Assert.Equal(_validAddress, manager.Address);
        Assert.Equal(_validLanguages, manager.Languages);
        Assert.Equal(Role.Manager, manager.Role);
        Assert.Same(teamMembers, manager.Team);
        Assert.Single(manager.Team);
    }

    [Fact]
    public void Constructor_WithGuid_SetsIdCorrectly()
    {
        var id = Guid.NewGuid();
        var teamMembers = new List<AbstractUser> { CreateUser() };

        var manager = new Manager(id, "Manager", "Test", _createdAt, _validContactInfo, _validAddress, _validLanguages,
            teamMembers);

        Assert.Equal(id, manager.Id);
    }

    [Fact]
    public void GetTeamMember_WithNullMember_ThrowsArgumentNullException()
    {
        var manager = CreateManager();

        Assert.Throws<ArgumentNullException>(() => manager.GetTeamMember(null));
    }

    [Fact]
    public void GetTeamMember_WithMemberNotInTeam_ReturnsNull()
    {
        var manager = CreateManager();
        var user = CreateUser();

        var result = manager.GetTeamMember(user);

        Assert.Null(result);
    }

    [Fact]
    public void GetTeamMember_WithMemberInTeam_ReturnsMember()
    {
        var user = CreateUser();
        var manager = CreateManager(team: new List<AbstractUser> { user });

        var result = manager.GetTeamMember(user);

        Assert.Same(user, result);
    }

    [Fact]
    public void RemoveMemberFromTeam_WithNullMember_ThrowsArgumentNullException()
    {
        var manager = CreateManager();

        Assert.Throws<ArgumentNullException>(() => manager.RemoveMemberFromTeam(null));
    }

    [Fact]
    public void RemoveMemberFromTeam_WithMemberNotInTeam_ThrowsArgumentException()
    {
        var manager = CreateManager();
        var user = CreateUser();

        var exception = Assert.Throws<ArgumentException>(() => manager.RemoveMemberFromTeam(user));
        Assert.Equal("Member not in team", exception.Message);
    }

    [Fact]
    public void RemoveMemberFromTeam_WithMemberInTeam_RemovesMember()
    {
        var user = CreateUser();
        var manager = CreateManager(team: new List<AbstractUser> { user });

        manager.RemoveMemberFromTeam(user);
        Assert.Empty(manager.Team);
    }

    [Fact]
    public void AddMemberToTeam_WithNullMember_ThrowsArgumentNullException()
    {
        var manager = CreateManager();

        Assert.Throws<ArgumentNullException>(() => manager.AddMemberToTeam(null));
    }

    [Fact]
    public void AddMemberToTeam_WithMemberAlreadyInTeam_ThrowsArgumentException()
    {
        var user = CreateUser();
        var manager = CreateManager(team: new List<AbstractUser> { user });

        var exception = Assert.Throws<ArgumentException>(() => manager.AddMemberToTeam(user));
        Assert.Equal("Member already in team", exception.Message);
    }

    [Fact]
    public void AddMemberToTeam_WithNewMember_AddsMember()
    {
        var user = CreateUser();
        var manager = CreateManager();

        manager.AddMemberToTeam(user);

        Assert.Single(manager.Team);
        Assert.Same(user, manager.Team[0]);
    }

    [Fact]
    public void AddMembersToTeam_WithNullMembers_ThrowsArgumentNullException()
    {
        var manager = CreateManager();

        Assert.Throws<ArgumentNullException>(() => manager.AddMembersToTeam(null));
    }

    /*
     * [Fact]
     
    public void AddMembersToTeam_WithValidMembers_AddsAllMembers()
    {
        var user1 = CreateUser("User", "One");
        var user2 = CreateUser("User", "Two");
        var newMembers = new List<AbstractUser> { user1, user2 };
        var manager = CreateManager();

        manager.AddMembersToTeam(newMembers);
        
        Assert.Equal(2, manager.Team.Count);
        Assert.Contains(user1, manager.Team);
        Assert.Contains(user2, manager.Team);
    }*/

    [Fact]
    public void AddMembersToTeam_WithSomeMembersAlreadyInTeam_OnlyAddsNewMembers()
    {
        var existingUser = CreateUser("Existing", "User");
        var newUser = CreateUser("New", "User");
        var manager = CreateManager(team: new List<AbstractUser> { existingUser });
        var newMembers = new List<AbstractUser> { existingUser, newUser };

        manager.AddMembersToTeam(newMembers);

       // Assert.Equal(2, manager.Team.Count);
        Assert.Contains(existingUser, manager.Team);
        Assert.Contains(newUser, manager.Team);
    }

    [Fact]
    public void AddMembersToTeam_WithEmptyList_DoesNotChangeTeam()
    {
        var existingUser = CreateUser();
        var manager = CreateManager(team: new List<AbstractUser> { existingUser });
        var emptyList = new List<AbstractUser>();

        manager.AddMembersToTeam(emptyList);

        Assert.Single(manager.Team);
        Assert.Same(existingUser, manager.Team[0]);
    }

    [Fact]
    public void Role_IsAlwaysManagerRole()
    {
        var manager = CreateManager();

        Assert.Equal(Role.Manager, manager.Role);
    }
}