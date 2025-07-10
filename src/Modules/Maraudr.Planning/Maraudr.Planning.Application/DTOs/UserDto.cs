namespace Maraudr.Planning.Application;

    public class UserDto
    {
        public Guid ManagerId { get; set; }
        public Guid? Manager { get; set; }  
        public int Role { get; set; }
        public Guid Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLoggedIn { get; set; }
        public bool IsActive { get; set; }
        public ContactInfoDto ContactInfo { get; set; }
        public AddressDto Address { get; set; }
        public string PasswordHash { get; set; }
        public List<object> Disponibilities { get; set; }  
        public string? Biography { get; set; }
        public string UserType { get; set; }
        public List<int> Languages { get; set; }
        public string RowVersion { get; set; }
    }

    public class ContactInfoDto
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class AddressDto
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
