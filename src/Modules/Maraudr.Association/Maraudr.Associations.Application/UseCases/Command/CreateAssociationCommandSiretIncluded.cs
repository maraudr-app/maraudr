using System.Text.Json;
using System.Text.Json.Serialization;
using Maraudr.Associations.Domain.Entities;
using Maraudr.Associations.Domain.Interfaces;
using Maraudr.Associations.Domain.Siret;
using Maraudr.Associations.Domain.ValueObjects;

namespace Maraudr.Associations.Application.UseCases.Command;

public interface ICreateAssociationHandlerSiretIncluded
{
    Task <Guid> HandleAsync(string siret, IHttpClientFactory factory);
}

public class CreateAssociationSiretIncluded(IAssociations associations) : ICreateAssociationHandlerSiretIncluded
{
    public async Task<Guid> HandleAsync(string siret, IHttpClientFactory factory)
    {
        using var client = factory.CreateClient("siret");
        var response = await client.GetAsync($"api/structure/{siret}");

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"SIRET API error: {response.StatusCode}");
        }

        var contentString = await response.Content.ReadAsStringAsync();

        if (contentString.Contains("statusCode: 404"))
        {
            throw new Exception("SIRET not found");
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var data = JsonSerializer.Deserialize<SiretApiResponse>(contentString, options);

        if (data is null)
        {
            throw new Exception("Invalid response from SIRET API");
        }

        if (data.Identity.LegalFormLabel != "Association déclarée")
        {
            throw new Exception("Le SIRET ne correspond pas à une association déclarée.");
        }

        var name = data.Identity.Nom;
        var addr = data.Coordinates.HeadquartersAddress;
        var fullStreet = $"{addr.NumVoie} {addr.TypeVoie} {addr.Voie}";
        var address = new Address(fullStreet, addr.Commune, addr.CodePostal.ToString(), "France");

        var association = new Association(name, addr.Commune, "France", new SiretNumber(siret), address);

        var result = await associations.RegisterAssociation(association);
        return result.Id;
    }
}

public record SiretApiResponse(
    [property: JsonPropertyName("identite")] Identity Identity,
    [property: JsonPropertyName("coordonnees")] Coordinates Coordinates
);

public record Identity(
    [property: JsonPropertyName("nom")]
    [property: JsonConverter(typeof(FlexibleStringConverter))]
    string Nom,

    [property: JsonPropertyName("lib_forme_juridique")]
    string LegalFormLabel
);

public record Coordinates(
    [property: JsonPropertyName("adresse_siege")] HeadquartersAddress HeadquartersAddress
);

public record HeadquartersAddress(
    [property: JsonPropertyName("num_voie")] int NumVoie,
    [property: JsonPropertyName("type_voie")] string TypeVoie,
    [property: JsonPropertyName("voie")] string Voie,
    [property: JsonPropertyName("cp")] int CodePostal,
    [property: JsonPropertyName("commune")] string Commune
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
