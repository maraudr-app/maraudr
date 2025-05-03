using Maraudr.Domain.ValueObjects;
using Maraudr.User.Domain.Entities;

namespace Maraudr.User.Infrastructure;

public static class DbSeeder
{
    public static async Task SeedAsync(UserContext context)
    {
        if (!context.Managers.Any())
        {
            var managers = new List<Manager>
            {
                new Manager(
                    "Marie", "Laurent", DateTime.Now.AddYears(-1),
                    new ContactInfo("marie.laurent@example.com", "+33698765432"),
                    new Address("45 Avenue Victor Hugo", "Lyon", "Auvergne-Rhône-Alpes", "69002", "France"),
                    new List<Language> { Language.English },new List<AbstractUser>{}
                ),
                new Manager(
                    "Sophie", "Bernard", DateTime.Now.AddMonths(-3),
                    new ContactInfo("sophie.bernard@example.com", "+33687654321"),
                    new Address("12 Rue Paradis", "Marseille", "Provence-Alpes-Côte d'Azur", "13001", "France"),
                    new List<Language> { Language.French, Language.German },new List<AbstractUser>{}
                )
            };

            context.Managers.AddRange(managers);
            await context.SaveChangesAsync();
        }
    }
}