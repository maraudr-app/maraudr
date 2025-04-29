namespace Maraudr.Stock.Infrastructure;

public class StockContext(DbContextOptions<StockContext> options) : DbContext(options)
{
    public DbSet<StockItem> Items { get; init; } = null!;
}
