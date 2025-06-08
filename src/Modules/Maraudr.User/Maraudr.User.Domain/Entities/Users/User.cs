using System.ComponentModel.DataAnnotations.Schema;
using Maraudr.User.Domain.ValueObjects.Users;

namespace Maraudr.User.Domain.Entities.Users
{

    public class User : AbstractUser
    {

        public override Role Role { get; protected set; } = Role.Member;

        public Guid? ManagerId { get; set; }

        [ForeignKey(nameof(ManagerId))]
        public Manager? Manager { get; set; }

        public User(string firstname, string lastname, DateTime createdAt,
            ContactInfo contactInfo, Address address, List<Language> languages,
            AbstractUser manager, string passwordHash)
            : base( firstname, lastname, createdAt, contactInfo, address, languages,passwordHash)
        {
            if (!manager.IsUserManager())
            {
                throw new ArgumentException("User role should be manager");
            }
            this.Manager = (Manager)manager;
        }
        
        public User(Guid id,string firstname, string lastname, DateTime createdAt,
            ContactInfo contactInfo, Address address, List<Language> languages,
            Manager manager)
            : base( id,firstname, lastname, createdAt, contactInfo, address, languages)
        {
            this.Manager = manager;
        }

        public User() { }

        public void ChangeManager(AbstractUser manager)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            if (manager.Role != Role.Manager)
                throw new ArgumentException("The user should already be a manager");
            Manager cManager = (Manager)manager;
            Manager.RemoveMemberFromTeam(this);
            cManager.AddMemberToTeam(this);
            Manager = cManager;
        }
        //TODO : réflechir aux règles metiers 
        //TODO: QU'est ce qu'un bénévole a de plus ?
        
    }

}