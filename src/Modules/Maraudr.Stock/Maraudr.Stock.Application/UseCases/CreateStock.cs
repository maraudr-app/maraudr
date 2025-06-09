namespace Maraudr.Stock.Application.UseCases;

public interface ICreateStockHandler
{
    Task<Guid> HandleAsync(Guid associationId);
}
public class CreateStock(IStockRepository repository) : ICreateStockHandler
{
    public async Task<Guid> HandleAsync(Guid associationId)
    {
        var stock = new Domain.Entities.Stock(associationId);
        await repository.CreateStockAsync(stock);
        return stock.Id;
    }
}
