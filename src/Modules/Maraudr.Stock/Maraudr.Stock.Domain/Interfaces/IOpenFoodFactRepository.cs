namespace Maraudr.Stock.Domain.Interfaces;

public interface IOpenFoodFactRepository
{
    Task<StockItem?> GetAllProductDataByCode(string code);
}