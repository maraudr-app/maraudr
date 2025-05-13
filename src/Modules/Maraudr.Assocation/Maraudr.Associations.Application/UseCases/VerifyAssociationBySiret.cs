namespace Maraudr.Associations.Application;

public class VerifyAssociationBySiret(IHttpClientFactory factory)
{
    public async Task<bool> HandleAsync(string siret)
    {
        using var client =  factory.CreateClient("siret");
        var response = await client.GetAsync(siret);
        Console.WriteLine(response.StatusCode);
        Console.WriteLine(await response.Content.ReadAsStringAsync());
        return response.IsSuccessStatusCode;
    }
}