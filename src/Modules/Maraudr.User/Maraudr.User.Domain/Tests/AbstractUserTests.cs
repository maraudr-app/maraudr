using Maraudr.User.Domain.Entities;
using Maraudr.User.Domain.ValueObjects;

namespace Domain

{
    public class AbstractUserTests
    {
        private readonly ContactInfo _validContactInfo;
        private readonly Address _validAddress;
        private readonly List<Language> _validLanguages;
        private readonly DateTime _createdAt;

        public AbstractUserTests()
        {
            _validContactInfo = new ContactInfo("test@example.com", "1234567890");
            _validAddress = new Address("123 Main St", "Anytown", "State", "12345", "Country");
            _validLanguages = new List<Language> { Language.English, Language.French };
            _createdAt = DateTime.Now.AddDays(-10);
        }

        private User CreateUser(string firstName = "John", string lastName = "Doe")
        {
            var manager = new Manager("Manager", "Test", _createdAt, _validContactInfo, _validAddress, _validLanguages, new List<AbstractUser>());
            return new User(firstName, lastName, _createdAt, _validContactInfo, _validAddress, _validLanguages, manager);
        }

        private Manager CreateManager(string firstName = "Manager", string lastName = "Test")
        {
            return new Manager(firstName, lastName, _createdAt, _validContactInfo, _validAddress, _validLanguages, new List<AbstractUser>());
        }

        [Fact]
        public void Constructor_WithValidParameters_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var user = CreateUser();

            // Assert
            Assert.Equal("John", user.Firstname);
            Assert.Equal("Doe", user.Lastname);
            Assert.Equal(_createdAt, user.CreatedAt);
            Assert.True(user.IsActive);
            Assert.Equal(_validContactInfo, user.ContactInfo);
            Assert.Equal(_validAddress, user.Address);
            Assert.Equal(_validLanguages, user.Languages);
            Assert.Equal(Role.Member, user.Role);
        }

        [Fact]
        public void Constructor_WithGuid_SetsIdCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var manager = CreateManager();

            // Act
            var user = new User(id, "John", "Doe", _createdAt, _validContactInfo, _validAddress, _validLanguages, manager);

            // Assert
            Assert.Equal(id, user.Id);
        }

        [Fact]
        public void IsUserManager_ForUser_ReturnsFalse()
        {
            var user = CreateUser();

            Assert.False(user.isUserManager());
        }

        [Fact]
        public void IsUserManager_ForManager_ReturnsTrue()
        {
            var manager = CreateManager();
            Assert.True(manager.isUserManager());
        }

        [Fact]
        public void IsUserAdmin_Default_ReturnsFalse()
        {
            var user = CreateUser();
            Assert.False(user.IsUserAdmin());
        }

        [Fact]
        public void GrantAdminPrivileges_ByNonAdmin_ThrowsException()
        {
            var user = CreateUser();
            var nonAdmin = CreateUser("Non", "Admin");
            var exception = Assert.Throws<UnauthorizedAccessException>(() => user.GrantAdminPrivileges(nonAdmin));
            Assert.Equal("Only admins can grant admin privileges", exception.Message);
        }

        [Fact]
        public void GrantAdminPrivileges_ByAdmin_SetsIsAdminTrue()
        {
            var user = CreateUser();
            var admin = CreateUser("Admin", "User");
            
            var isAdminProperty = typeof(AbstractUser).GetProperty("IsAdmin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            isAdminProperty.SetValue(admin, true);
            user.GrantAdminPrivileges(admin);
            Assert.True(user.IsUserAdmin());
        }

        [Fact]
        public void RevokeAdminPrivileges_ByNonAdmin_ThrowsException()
        {
            var user = CreateUser();
            var nonAdmin = CreateUser("Non", "Admin");
            var isAdminProperty = typeof(AbstractUser).GetProperty("IsAdmin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            isAdminProperty.SetValue(user, true);
            var exception = Assert.Throws<UnauthorizedAccessException>(() => user.RevokeAdminPrivileges(nonAdmin));
            Assert.Equal("Only admins can revoke admin privileges", exception.Message);
        }

        [Fact]
        public void RevokeAdminPrivileges_ByAdmin_SetsIsAdminFalse()
        {
            var user = CreateUser();
            var admin = CreateUser("Admin", "User");
            var isAdminProperty = typeof(AbstractUser).GetProperty("IsAdmin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            isAdminProperty.SetValue(user, true);
            isAdminProperty.SetValue(admin, true);
            user.RevokeAdminPrivileges(admin);
            Assert.False(user.IsUserAdmin());
        }

        [Fact]
        public void ChangeUserRole_ByNonAdmin_ThrowsException()
        {
            var admin = CreateUser();
            var targetUser = CreateUser("Target", "User");
            var newManager = CreateManager();
            var exception = Assert.Throws<UnauthorizedAccessException>(() => admin.ChangeUserRole(targetUser, Role.Manager, newManager));
            Assert.Equal("Only admins can change user roles", exception.Message);
        }
        
        [Fact]
        public void ChangeUserRole_FromMemberToManager_ReturnsManager()
        {
            var admin = CreateUser("Admin", "User");
            var targetUser = CreateUser("Target", "User");
            var newManager = CreateManager();
            
            var isAdminProperty = typeof(AbstractUser).GetProperty("IsAdmin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            isAdminProperty.SetValue(admin, true);

            var result = admin.ChangeUserRole(targetUser, Role.Manager, newManager);

            Assert.IsType<Manager>(result);
            Assert.Equal(Role.Manager, result.Role);
            Assert.Equal(targetUser.Id, result.Id);
            Assert.Equal(targetUser.Firstname, result.Firstname);
            Assert.Equal(targetUser.Lastname, result.Lastname);
        }

        [Fact]
        public void ChangeUserRole_FromManagerToMember_ReturnsUser()
        {
            var admin = CreateUser("Admin", "User");
            var targetManager = CreateManager("Target", "Manager");
            var newManager = CreateManager();
            
            var isAdminProperty = typeof(AbstractUser).GetProperty("IsAdmin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            isAdminProperty.SetValue(admin, true);

            var result = admin.ChangeUserRole(targetManager, Role.Member, newManager);

            Assert.IsType<User>(result);
            Assert.Equal(Role.Member, result.Role);
            Assert.Equal(targetManager.Id, result.Id);
            Assert.Equal(targetManager.Firstname, result.Firstname);
            Assert.Equal(targetManager.Lastname, result.Lastname);
        }

        [Fact]
        public void ChangeUserRole_FromManagerToMember_WithNonManagerNewManager_ThrowsException()
        {
            // Arrange
            var admin = CreateUser("Admin", "User");
            var targetManager = CreateManager("Target", "Manager");
            var nonManager = CreateUser("Non", "Manager"); // This is a User, not a Manager
            
            // Make admin an actual admin
            var isAdminProperty = typeof(AbstractUser).GetProperty("IsAdmin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            isAdminProperty.SetValue(admin, true);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => admin.ChangeUserRole(targetManager, Role.Member, nonManager));
        }

        [Fact]
        public void Equals_WithSameInstance_ReturnsTrue()
        {
            // Arrange
            var user = CreateUser();

            // Act & Assert
            Assert.True(user.Equals(user));
        }

        [Fact]
        public void Equals_WithNull_ReturnsFalse()
        {
            // Arrange
            var user = CreateUser();

            // Act & Assert
            Assert.False(user.Equals(null));
        }

        [Fact]
        public void Equals_WithDifferentType_ReturnsFalse()
        {
            // Arrange
            var user = CreateUser();
            var notUser = new object();

            // Act & Assert
            Assert.False(user.Equals(notUser));
        }

        [Fact]
        public void Equals_WithSameId_ReturnsTrue()
        {
            var id = Guid.NewGuid();
            var manager = CreateManager();
            var user1 = new User(id, "John", "Doe", _createdAt, _validContactInfo, _validAddress, _validLanguages, manager);
            var user2 = new User(id, "Different", "Name", DateTime.Now, new ContactInfo("other@example.com", "9876543210"),
                new Address("Different St", "Other City", "Other State", "54321", "Other Country"),
                new List<Language> { Language.German }, manager);

            Assert.True(user1.Equals(user2));
        }
    }
}