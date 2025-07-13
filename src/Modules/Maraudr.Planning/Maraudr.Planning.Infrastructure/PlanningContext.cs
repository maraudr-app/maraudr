

using Maraudr.Planning.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Maraudr.Planning.Infrastructure
{
    public class PlanningContext : DbContext
    {
        public DbSet<Domain.Entities.Planning> Plannings { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;

        public PlanningContext(DbContextOptions<PlanningContext> options) : base(options) { }
        
        public PlanningContext() : base(GetDefaultOptions()) { }

        private static DbContextOptions<PlanningContext> GetDefaultOptions()
        {
            var builder = new DbContextOptionsBuilder<PlanningContext>();
            builder.UseNpgsql("Host=maraudr-db;Port=5432;Database=maraudr-dev;Username=postgres;Password=postgres");
            return builder.Options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.Entities.Planning>(builder =>
            {
                builder.HasKey(p => p.Id);
                builder.Property(p => p.AssociationId).IsRequired();
                
                builder.HasMany(p => p.Events)
                      .WithOne()
                      .HasForeignKey(e => e.PlanningId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Event>(builder =>
            {
                builder.HasKey(e => e.Id);
                builder.Property(e => e.PlanningId).IsRequired();
                builder.Property(e => e.OrganizerdId).IsRequired();
                builder.Property(e => e.Title).IsRequired();
                
                // Conversion des dates en UTC
                builder.Property(e => e.BeginningDate)
                      .HasConversion(
                          v => v.ToUniversalTime(),
                          v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                      );
                
                builder.Property(e => e.EndDate)
                      .HasConversion(
                          v => v.ToUniversalTime(),
                          v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                      );
                
                builder.Property(e => e.ParticipantsIds)
                      .HasColumnType("uuid[]");
            });
        }
    }
}