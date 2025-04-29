namespace Maraudr.Stock.Application.UseCases;

public interface ICreateItemHandler
{
    Task<Guid> HandleAsync(CreateItemCommand command);
}

public class CreateItemHandler(IStockRepository repository) : ICreateItemHandler
{
    public async Task<Guid> HandleAsync(CreateItemCommand command)
    {
        var item = new StockItem(command.Name, command.Description, command.ItemType);
        await repository.CreateStockItemAsync(item);
        return item.Id;
    }
}
