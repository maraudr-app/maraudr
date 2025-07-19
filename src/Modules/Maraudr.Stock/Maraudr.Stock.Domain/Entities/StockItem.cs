namespace Maraudr.Stock.Domain.Entities
{
    // todo: notify when quantites get in a red zone 
    public class StockItem : IResource
    {
        public Guid Id { get; init; }
        public Guid StockId { get; private set; }
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public string? BarCode { get; init; }
        public Category Category { get; init; }
        public DateTime EntryDate { get; init; } = DateTime.Now;

        public int Quantity { get; set; } = 1;

        public StockItem() { }

        public StockItem(string name,string description = null!, Category type = Category.Unknown)
        {
            Id = Guid.NewGuid();
            Name = name ?? throw new InvalidItemNameException("Item name is null");
            Description = description;
            Category = type;
        }
        
        public StockItem(string name, string description, string barCode, Category category, Guid stockId)
        {
            Name = name;
            Description = description;
            BarCode = barCode;
            Category = category;
            StockId = stockId;
        }
        
        

        public void RemoveAnItem(int quantity)
        {
            var i = !(Quantity <= quantity)?Quantity -= quantity:Quantity = 0;
        }

        public override bool Equals(object? obj)
        {
            if (obj?.GetType().Name != this.GetType().Name)
            {
                return false;
            }

            var cStockItem = (StockItem)obj;

            return cStockItem.Id == Id;
        }

        
    }
}
