﻿using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Maraudr.Associations.Application.Dtos;
using Maraudr.Associations.Domain.Entities;
using Maraudr.Associations.Domain.Interfaces;
using Maraudr.Associations.Domain.Siret;
using Maraudr.Associations.Domain.ValueObjects;

namespace Maraudr.Associations.Application.UseCases.Command;

public interface ICreateAssociationHandlerSiretIncluded
{
    Task<AssociationWithStockResponse> HandleAsync(
        string siret, Guid id,
        IHttpClientFactory siretHttpFactory,
        IHttpClientFactory stockHttpFactory,
        IHttpClientFactory geoHttpFactory);
}

public class CreateAssociationSiretIncluded(IAssociations associations) : ICreateAssociationHandlerSiretIncluded
{
    public async Task<AssociationWithStockResponse> HandleAsync(
    string siret, Guid id,
    IHttpClientFactory siretHttpFactory,
    IHttpClientFactory stockHttpFactory,
    IHttpClientFactory geoHttpFactory)
{
    using var siretClient = siretHttpFactory.CreateClient("siret");
    var response = await siretClient.GetAsync($"api/structure/{siret}");

    if (!response.IsSuccessStatusCode)
        throw new Exception($"SIRET API error: {response.StatusCode}");

    var contentString = await response.Content.ReadAsStringAsync();

    if (contentString.Contains("statusCode: 404"))
        throw new Exception("SIRET not found");

    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var data = JsonSerializer.Deserialize<SiretApiResponse>(contentString, options);

    if (data is null)
        throw new Exception("Invalid response from SIRET API");

    if (data.Identity.LegalFormLabel != "Association déclarée")
        throw new Exception("Le SIRET ne correspond pas à une association déclarée.");

    var name = data.Identity.Nom;
    var addr = GetBestAddress(data.Coordinates);

    var fullStreet = string.Join(" ", new[] { addr.NumVoie, addr.TypeVoie, addr.Voie }
        .Where(part => !string.IsNullOrWhiteSpace(part)));

    var address = new Address(fullStreet, addr.Commune, addr.CodePostal.ToString(), "France");

    var association = new Association(name, addr.Commune, "France", new SiretNumber(siret), address)
    {
        ManagerId = id
    };
    association.Members.Add(id);

    var result = await associations.RegisterAssociation(association);
    if (result is null)
        throw new Exception("Failed to create association.");

    using var stockClient = stockHttpFactory.CreateClient("stock");
    
    stockClient.DefaultRequestHeaders.Add("X-Stock-Api-Key", Environment.GetEnvironmentVariable("STOCK_API_KEY"));
    Console.WriteLine($"Envoi à /create-stock avec AssociationId = {result.Id}");
    Console.WriteLine($"Stock endpoint: {stockClient.BaseAddress}");
    Console.WriteLine(Environment.GetEnvironmentVariable("STOCK_API_KEY"));

    var stockResponse = await stockClient.PostAsJsonAsync("/create-stock", new { AssociationId = result.Id });
    if (!stockResponse.IsSuccessStatusCode)
        throw new Exception("Stock creation failed");

    var stockData = await stockResponse.Content.ReadFromJsonAsync<StockResponse>();
    if (stockData == null)
        throw new Exception("Invalid response from stock API");
    
    using var planningClient = stockHttpFactory.CreateClient("planning");

    planningClient.DefaultRequestHeaders.Add("X-Api-Key", Environment.GetEnvironmentVariable("ASSOCIATION_API_KEY"));

    Console.WriteLine($"Envoi à /create-planning avec AssociationId = {result.Id}");
    Console.WriteLine($"Stock endpoint: {planningClient.BaseAddress}");
    Console.WriteLine(Environment.GetEnvironmentVariable("ASSOCIATION_API_KEY"));
    
    var planningResponse = await planningClient.PostAsJsonAsync("/api/planning/create-planning", new { AssociationId = result.Id });

    if (!planningResponse.IsSuccessStatusCode)
        throw new Exception($"Planning creation failed: {planningResponse.StatusCode}");

    var planningData = await planningResponse.Content.ReadFromJsonAsync<PlanningResponse>();
    if (planningData == null)
        throw new Exception("Invalid response from planning API");

    using var geoClient = geoHttpFactory.CreateClient("geo"); 
    
    geoClient.DefaultRequestHeaders.Add("X-Geo-Api-Key", Environment.GetEnvironmentVariable("GEO_API_KEY"));
    
    var geoResponse = await geoClient.PostAsJsonAsync("/geo/store", new CreateGeoStoreRequest(result.Id));
    if (!geoResponse.IsSuccessStatusCode)
        throw new Exception("GeoStore creation failed");

    var geoData = await geoResponse.Content.ReadFromJsonAsync<GeoStoreResponse>();
    if (geoData == null)
        throw new Exception("Invalid response from geo API");

    return new AssociationWithStockResponse(result.Id, stockData.Id, geoData.Id, planningData.Id);}
    
    private static HeadquartersAddress GetBestAddress(Coordinates coords)
    {
        var primary = coords.HeadquartersAddress;
        if (string.IsNullOrWhiteSpace(primary.NumVoie) && coords.HeadquartersAddressSirene is not null)
        {
            return coords.HeadquartersAddressSirene;
        }
        return primary;
    }
}

public record AssociationWithStockResponse(Guid AssociationId, Guid StockId, Guid GeoStoreId,Guid PlanningId);
public record StockResponse(Guid Id);
public record PlanningResponse(Guid Id);
public record CreateGeoStoreRequest(Guid AssociationId);
public record SiretApiResponse(
    [property: JsonPropertyName("identite")] Identity Identity,
    [property: JsonPropertyName("coordonnees")] Coordinates Coordinates
);

public record Identity(
    [property: JsonPropertyName("nom")]
    [property: JsonConverter(typeof(FlexibleStringConverter))] string Nom,

    [property: JsonPropertyName("lib_forme_juridique")]
    string LegalFormLabel
);

public record Coordinates(
    [property: JsonPropertyName("adresse_siege")] HeadquartersAddress HeadquartersAddress,
    [property: JsonPropertyName("adresse_siege_sirene")] HeadquartersAddress? HeadquartersAddressSirene
);

public record HeadquartersAddress(
    [property: JsonPropertyName("num_voie")]
    [property: JsonConverter(typeof(FlexibleStringConverter))] string NumVoie,

    [property: JsonPropertyName("type_voie")]
    string TypeVoie,

    [property: JsonPropertyName("voie")]
    string Voie,

    [property: JsonPropertyName("cp")]
    int CodePostal,

    [property: JsonPropertyName("commune")]
    string Commune
);

public class FlexibleStringConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.Number => reader.GetInt32().ToString(),
            _ => throw new JsonException($"Unexpected token {reader.TokenType}")
        };
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
