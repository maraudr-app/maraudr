using Maraudr.Authentication.Domain.Entities;
using Maraudr.User.Domain.Entities.Tokens;
using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Domain.ValueObjects.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Maraudr.User.Infrastructure;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options) { }

    public DbSet<AbstractUser> Users { get; set; }
    public DbSet<RefreshToken?> RefreshTokens { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<AbstractUser>()
            .HasDiscriminator<string>("UserType")
            .HasValue<Manager>("Manager")
            .HasValue<Domain.Entities.Users.User>("User");
        // --- Value Objects ---
        modelBuilder.Entity<AbstractUser>().OwnsOne(m => m.Address);
        modelBuilder.Entity<AbstractUser>().OwnsOne(m => m.ContactInfo);

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
                        // Pour déboguer, enlevez le .FirstOrDefault() et vérifiez si des utilisateurs sont retournés
        modelBuilder.Entity<AbstractUser>().OwnsOne(u => u.ContactInfo);
    
                        // Configuration spécifique aux managers
        modelBuilder.Entity<AbstractUser>().OwnsOne(m => m.Address);
        
        modelBuilder.Entity<AbstractUser>()
            .Property(m => m.Languages)
            .HasConversion(languageConverter);
        modelBuilder.Entity<RefreshToken>()
            .HasKey(r => r.Id);
            
        modelBuilder.Entity<RefreshToken>()
            .Property(r => r.Token)
            .IsRequired();
            
        modelBuilder.Entity<RefreshToken>()
            .Property(r => r.ExpiresAt)
            .IsRequired();
            
        modelBuilder.Entity<RefreshToken>()
            .Property(r => r.UserId)
            .IsRequired();
        
        modelBuilder.Entity<RefreshToken>()
            .HasOne<AbstractUser>() 
            .WithMany() // Un utilisateur peut avoir plusieurs refresh tokens
            .HasForeignKey(r => r.UserId) // Clé étrangère
            .OnDelete(DeleteBehavior.Cascade); 
    
        
        
    }
}