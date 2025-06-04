using Microsoft.Extensions.Options;

namespace Maraudr.Stock.Infrastructure.Repository;

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class OpenFoodFactRepository(HttpClient httpClient,IOptions<ApiSettings> options) : IOpenFoodFactRepository
{
    public async Task<StockItem?> GetAllProductDataByCode(string code)
    {
        var url = options.Value.OpenFoodFactApiUrl + $"product/{code}.json";
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);

        if (!document.RootElement.TryGetProperty("product", out var product)) return null;

        return new StockItem
        {
            Name = product.GetProperty("product_name").GetString() ?? "Unknown",
            Description = product.TryGetProperty("ingredients_text", out var desc) ? desc.GetString() : null,
            BarCode = code,
            Category = Category.Food
        };
    }
}