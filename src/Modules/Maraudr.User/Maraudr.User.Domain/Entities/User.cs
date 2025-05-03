
using Maraudr.Domain;
using Maraudr.Domain.ValueObjects;
using Maraudr.User.Domain;
using Maraudr.User.Domain.Entities;

namespace Maraudr.User.Domain.Entities
{

    public class User : AbstractUser
    {

        public override AccountType AccountType { get; protected set; } = AccountType.Member;

        public Manager Manager { get; set; }

        public User(string firstname, string lastname, DateTime createdAt,
            ContactInfo contactInfo, Address address, List<Language> languages,
            Manager manager)
            : base( firstname, lastname, createdAt, contactInfo, address, languages)
        {
            this.Manager = manager;
        }
        
        public User(Guid id,string firstname, string lastname, DateTime createdAt,
            ContactInfo contactInfo, Address address, List<Language> languages,
            Manager manager)
            : base( id,firstname, lastname, createdAt, contactInfo, address, languages)
        {
            this.Manager = manager;
        }

        public void ChangeManager(AbstractUser manager)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            if (manager.AccountType != AccountType.Manager)
                throw new ArgumentException("The user should already be a manager");
            Manager cManager = Manager;
            Manager.RemoveMemberFromTeam(this);
            cManager.AddMemberToTeam(this);
            Manager = cManager;
        }
        //TODO : réflechir aux règles metiers 
        //TODO: QU'est ce qu'un bénévole a de plus ?
    }

}