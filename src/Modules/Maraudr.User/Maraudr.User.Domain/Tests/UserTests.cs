using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Domain.ValueObjects.Users;

namespace Domain
{
    public class UserTests
    {
        private readonly ContactInfo _validContactInfo;
        private readonly Address _validAddress;
        private readonly List<Language> _validLanguages;
        private readonly DateTime _createdAt;

        public UserTests()
        {
            _validContactInfo = new ContactInfo("test@example.com", "1234567890");
            _validAddress = new Address("123 Main St", "Anytown", "State", "12345", "Country");
            _validLanguages = new List<Language> { Language.English, Language.French };
            _createdAt = DateTime.Now.AddDays(-10);
        }

        private Manager CreateManager(string firstName = "Manager", string lastName = "Test")
        {
            return new Manager(firstName, lastName, _createdAt, _validContactInfo, _validAddress, _validLanguages, new List<AbstractUser>());
        }

        [Fact]
        public void Constructor_WithValidParameters_SetsPropertiesCorrectly()
        {
            // Arrange
            var manager = CreateManager();

            // Act
            var user = new User("John", "Doe", _createdAt, _validContactInfo, _validAddress, _validLanguages, manager);

            // Assert
            Assert.Equal("John", user.Firstname);
            Assert.Equal("Doe", user.Lastname);
            Assert.Equal(_createdAt, user.CreatedAt);
            Assert.True(user.IsActive);
            Assert.Equal(_validContactInfo, user.ContactInfo);
            Assert.Equal(_validAddress, user.Address);
            Assert.Equal(_validLanguages, user.Languages);
            Assert.Equal(Role.Member, user.Role);
            Assert.Same(manager, user.Manager);
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
        public void Role_IsAlwaysMemberRole()
        {
            // Arrange
            var manager = CreateManager();

            // Act
            var user = new User("John", "Doe", _createdAt, _validContactInfo, _validAddress, _validLanguages, manager);

            // Assert
            Assert.Equal(Role.Member, user.Role);
        }

        [Fact]
        public void ChangeManager_WithValidManager_ChangesManager()
        {
            // Arrange
            var initialManager = CreateManager("Initial", "Manager");
            var newManager = CreateManager("New", "Manager");
            var user = new User("John", "Doe", _createdAt, _validContactInfo, _validAddress, _validLanguages, initialManager);
            initialManager.AddMemberToTeam(user);
            user.ChangeManager(newManager);

            Assert.Same(newManager, user.Manager);
        }

        [Fact]
        public void ChangeManager_WithNullManager_ThrowsArgumentNullException()
        {
            // Arrange
            var initialManager = CreateManager();
            var user = new User("John", "Doe", _createdAt, _validContactInfo, _validAddress, _validLanguages, initialManager);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => user.ChangeManager(null));
        }

        [Fact]
        public void ChangeManager_WithNonManagerUser_ThrowsArgumentException()
        {
            // Arrange
            var initialManager = CreateManager();
            var user = new User("John", "Doe", _createdAt, _validContactInfo, _validAddress, _validLanguages, initialManager);
            
            // Create a Non-Manager AbstractUser by creating a User
            var nonManager = new User("Non", "Manager", _createdAt, _validContactInfo, _validAddress, _validLanguages, initialManager);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => user.ChangeManager(nonManager));
        }
    }
}