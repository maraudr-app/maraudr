using FluentAssertions;
using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Domain.ValueObjects.Users;

namespace UserTests;

public class UserTests
{
    [Fact]
    public void AddMemberToTeam_WithNewMember_ShouldAddSuccessfully_Manager()
    {
        // Arrange
        var manager = CreateTestManager();
        var user = CreateTestUser();

        // Act
        manager.AddMemberToTeam(user);

        // Assert
        manager.Team.Should().Contain(user);
    }

    [Fact]
    public void AddMemberToTeam_WithExistingMember_ShouldThrowArgumentException_Manager()
    {
        // Arrange
        var manager = CreateTestManager();
        var user = CreateTestUser();
        manager.AddMemberToTeam(user);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => manager.AddMemberToTeam(user));
    }

    [Fact]
    public void RemoveMemberFromTeam_WithExistingMember_ShouldRemoveSuccessfully_Manager()
    {
        // Arrange
        var manager = CreateTestManager();
        var user = CreateTestUser();
        manager.AddMemberToTeam(user);

        // Act
        manager.RemoveMemberFromTeam(user);

        // Assert
        manager.Team.Should().NotContain(user);
    }

    private Manager CreateTestManager()
    {
        var contactInfo = new ContactInfo("manager@example.com", "123456789");
        var address = new Address("Street", "City", "State", "12345", "Country");
        return new Manager("Manager", "Test", DateTime.Now, contactInfo, address, new List<Language>(), new List<AbstractUser>(), "hashedPassword");
    }

    private User CreateTestUser()
    {
        var contactInfo = new ContactInfo("user@example.com", "123456789");
        var address = new Address("Street", "City", "State", "12345", "Country");
        var manager = CreateTestManager();
        return new User("User", "Test", DateTime.Now, contactInfo, address, new List<Language>(), manager, "hashedPassword");
    }
    
    [Fact]
    public void AddMemberToTeam_WithNewMember_ShouldAddSuccessfully()
    {
        // Arrange
        var manager = CreateTestManager();
        var user = CreateTestUser();

        // Act
        manager.AddMemberToTeam(user);

        // Assert
        manager.Team.Should().Contain(user);
    }

    [Fact]
    public void AddMemberToTeam_WithExistingMember_ShouldThrowArgumentException()
    {
        // Arrange
        var manager = CreateTestManager();
        var user = CreateTestUser();
        manager.AddMemberToTeam(user);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => manager.AddMemberToTeam(user));
    }

    [Fact]
    public void RemoveMemberFromTeam_WithExistingMember_ShouldRemoveSuccessfully()
    {
        // Arrange
        var manager = CreateTestManager();
        var user = CreateTestUser();
        manager.AddMemberToTeam(user);

        // Act
        manager.RemoveMemberFromTeam(user);

        // Assert
        manager.Team.Should().NotContain(user);
    }

  
    
}