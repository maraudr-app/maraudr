
using Maraudr.User.Domain;
using Maraudr.User.Domain.Entities;
using Maraudr.User.Domain.ValueObjects;

namespace Maraudr.User.Domain.Entities
{

    public class User : AbstractUser
    {

        public override Role Role { get; protected set; } = Role.Member;

        public Manager Manager { get; set; }

        public User(string firstname, string lastname, DateTime createdAt,
            ContactInfo contactInfo, Address address, List<Language> languages,
            AbstractUser manager)
            : base( firstname, lastname, createdAt, contactInfo, address, languages)
        {
            if (!manager.isUserManager())
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