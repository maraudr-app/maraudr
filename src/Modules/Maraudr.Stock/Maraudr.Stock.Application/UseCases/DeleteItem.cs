namespace Maraudr.Stock.Application.UseCases
{
    public interface IDeleteItemHandler
    {
        Task HandleAsync(Guid id);
    }
    public class DeleteItemHandler(IStockRepository respository) : IDeleteItemHandler
    {
        public async Task HandleAsync(Guid id)
        {
            await respository.DeleteStockItemAsync(id);
        }
    }
}
