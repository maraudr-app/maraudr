namespace Maraudr.Stock.Domain.Entities
{
    public class StockItem : IResource
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public Category Category { get; init; }
        public DateTime EntryDate { get; init; } = DateTime.Now;

        public StockItem() { }

        public StockItem(string name, string description = null!, Category type = Category.Unknown)
        {
            Id = Guid.NewGuid();
            Name = name ?? throw new InvalidItemNameException("Item name is null");
            Description = description;
            Category = type;
        }
    }
}
