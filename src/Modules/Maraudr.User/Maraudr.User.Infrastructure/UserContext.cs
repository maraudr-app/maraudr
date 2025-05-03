using Maraudr.Domain.ValueObjects;
using Maraudr.User.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Maraudr.User.Infrastructure;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options) { }

    public DbSet<AbstractUser> Users { get; set; }
    public DbSet<Manager> Managers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Value Objects ---
        modelBuilder.Entity<Manager>().OwnsOne(m => m.Address);
        modelBuilder.Entity<Manager>().OwnsOne(m => m.ContactInfo);

        // --- Enum List Conversion ---
        var languageConverter = new ValueConverter<List<Language>, string>(
            v => string.Join(',', v.Select(e => e.ToString())),
            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => Enum.Parse<Language>(e))
                .ToList());

        var roleConverter = new ValueConverter<List<Role>, string>(
            v => string.Join(',', v.Select(e => e.ToString())),
            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => Enum.Parse<Role>(e))
                .ToList());

        modelBuilder.Entity<Manager>()
            .Property(m => m.Languages)
            .HasConversion(languageConverter);
        
    }
}