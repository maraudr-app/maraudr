using Maraudr.Stock.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Maraudr.Stock.Infrastructure;

public class StockContext : DbContext
{
    public DbSet<Domain.Entities.Stock> Stocks { get; set; } = null!;
    public DbSet<StockItem> Items { get; set; } = null!;

    public StockContext(DbContextOptions<StockContext> options) : base(options) { }

    public StockContext() : base(GetDefaultOptions()) { }

    private static DbContextOptions<StockContext> GetDefaultOptions()
    {
        var builder = new DbContextOptionsBuilder<StockContext>();
        builder.UseNpgsql("Host=localhost;Port=5432;Database=maraudr-stock;Username=postgres;Password=postgres");
        return builder.Options;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Entities.Stock>()
            .HasMany(s => s.Items)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StockItem>()
            .Property(i => i.EntryDate)
            .HasConversion(
                d => d.ToUniversalTime(),
                d => DateTime.SpecifyKind(d, DateTimeKind.Utc)
            );
    }
}