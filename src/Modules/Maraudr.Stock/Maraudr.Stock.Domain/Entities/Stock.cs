namespace Maraudr.Stock.Domain.Entities;

public class Stock
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid AssociationId { get; set; } 
    public List<StockItem> Items { get; set; } = [];

    public Stock() { }

    public Stock(Guid associationId)
    {
        AssociationId = associationId;
    }

    public void AddItem(StockItem item)
    {
        Items.Add(item);
    }

    public void RemoveItem(StockItem item)
    {
        Items.Remove(item);
    }
}