using Maraudr.Authentication.Domain.Entities;
using Maraudr.User.Domain.Entities.Tokens;
using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Domain.ValueObjects.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Maraudr.User.Infrastructure;

public class UserContext : DbContext
{
    public UserContext() { }
    public UserContext(DbContextOptions<UserContext> options) : base(options) { }
    public DbSet<AbstractUser> Users { get; set; }
    public DbSet<RefreshToken?> RefreshTokens { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=maraudr-dev;Username=postgres;Password=postgres");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AbstractUser>()
            .HasDiscriminator<string>("UserType")
            .HasValue<Domain.Entities.Users.User>("User")
            .HasValue<Manager>("Manager");

        modelBuilder.Entity<Domain.Entities.Users.User>(builder =>
        {
            builder.HasOne(u => u.Manager)
                .WithMany(m => m.EFTeam)
                .HasForeignKey(u => u.ManagerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        
        modelBuilder.Entity<AbstractUser>()
            .Property(u => u.CreatedAt)
            .HasConversion(
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );
        
        modelBuilder.Entity<AbstractUser>()
            .Property(u => u.LastLoggedIn)
            .HasConversion(
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );
        
        modelBuilder.Entity<AbstractUser>().OwnsOne(u => u.Address);
        modelBuilder.Entity<AbstractUser>().OwnsOne(u => u.ContactInfo);

        var languageConverter = new ValueConverter<List<Language>, string>(
            v => string.Join(',', v.Select(e => e.ToString())),
            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(e => Enum.Parse<Language>(e))
                  .ToList());

        var languageComparer = new ValueComparer<List<Language>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        modelBuilder.Entity<AbstractUser>()
            .Property(m => m.Languages)
            .HasConversion(languageConverter, languageComparer);

        var roleConverter = new ValueConverter<Role, string>(
            v => v.ToString(),
            v => Enum.Parse<Role>(v));

        modelBuilder.Entity<AbstractUser>()
            .Property(u => u.Role)
            .HasConversion(roleConverter);

        modelBuilder.Entity<RefreshToken>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<RefreshToken>()
            .Property(r => r.Token).IsRequired();

        modelBuilder.Entity<RefreshToken>()
            .Property(r => r.ExpiresAt).IsRequired();

        modelBuilder.Entity<RefreshToken>()
            .Property(r => r.UserId).IsRequired();

        modelBuilder.Entity<RefreshToken>()
            .HasOne<AbstractUser>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.UserId).IsRequired();
            entity.Property(p => p.Token).IsRequired().HasMaxLength(500);
            entity.Property(p => p.ExpiresAt).IsRequired();

            entity.HasOne<AbstractUser>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
